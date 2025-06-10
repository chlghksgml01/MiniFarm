using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum playerDir
{
    Up,
    Down,
    Left,
    Right
}

public class Player : Entity
{
    [SerializeField] public int maxHp;
    [SerializeField] public int maxStamina;

    [SerializeField] public int workStaminaCost = 5;
    [SerializeField] public PlayerSaveData playerSaveData = new PlayerSaveData();

    [Header("Item")]
    [SerializeField] public HoldItem holdItem;

    [Header("Sword")]
    [SerializeField] public GameObject SwordColliderRight;
    [SerializeField] public GameObject SwordColliderLeft;
    [SerializeField] public GameObject SwordColliderDown;
    [SerializeField] public GameObject SwordColliderUp;

    [Header("UI")]
    [SerializeField] public Image healthBar;
    [SerializeField] public Image staminaBar;

    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public Animator anim;

    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerWorkingState workingState;
    [HideInInspector] public PlayerPickUpState pickUpState;

    public PlayerStateMachine stateMachine { get; private set; }

    public ToolType playerToolType { get; set; } = ToolType.None;
    public playerDir playerDir { get; set; } = playerDir.Down;

    public bool isDead = false;
    private SFXManager sfxManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.Log("Player - anim 없음");

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "isMoving");
        moveState = new PlayerMoveState(this, stateMachine, "isMoving");
        workingState = new PlayerWorkingState(this, stateMachine, "isWorking");
        pickUpState = new PlayerPickUpState(this, stateMachine, "isPickingUp");

        stateMachine.Initialize(idleState);

        playerSaveData.inventory = new Inventory(GameManager.Instance.uiManager.inventory_UI.slotsUIs.Count);
    }
    private void Start()
    {
        sfxManager = SoundManager.Instance.sfxManager;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.dayTimeManager.OnDayFinished += PrepareNewDay;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            return;
        GameManager.Instance.dayTimeManager.OnDayFinished -= PrepareNewDay;
    }

    private void PrepareNewDay()
    {
        playerSaveData.hp = maxHp;

        if (!isDead)
            playerSaveData.stamina = maxStamina;
        else
            playerSaveData.stamina = maxStamina * 3 / 4;

        SetGague(staminaBar, playerSaveData.stamina, maxStamina);

        isDead = false;
        anim.SetBool("isDeath", false);
        stateMachine.ChangeState(idleState);
        moveInput = Vector3.zero;
    }

    void Update()
    {
        if (isDead || GameManager.Instance.uiManager.IsUIOpen() || SceneLoadManager.Instance.isSceneLoading)
        {
            if (sfxManager.isPlaying())
                sfxManager.Stop();
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 단위벡터-> 대각선으로 가도 같은 속도로 이동하게끔
        moveInput = moveInput.normalized;

        if (moveInput.magnitude != 0 && !SoundManager.Instance.sfxManager.isPlaying())
        {
            if (SceneManager.GetActiveScene().name == "Farm")
                sfxManager.Play(sfxManager.farmFootsteps);
            else if (SceneManager.GetActiveScene().name == "House")
                sfxManager.Play(sfxManager.houseFootsteps);
        }
        else if (moveInput.magnitude == 0 && SoundManager.Instance.sfxManager.isPlaying())
            sfxManager.Stop();

        if (stateMachine.currentState != workingState && stateMachine.currentState != pickUpState)
        {
            stateMachine.currentState.UpdateState();
        }
    }

    private void FixedUpdate()
    {
        if (stateMachine.currentState != workingState && stateMachine.currentState != pickUpState)
            transform.Translate(moveInput * speed * Time.fixedDeltaTime);
    }

    public void Damage(int _damage)
    {
        playerSaveData.hp -= _damage;
        SetGague(healthBar, playerSaveData.hp, maxHp);
        if (playerSaveData.hp <= 0 && !isDead)
        {
            Die();
        }
        StartCoroutine(FlashFX());
    }

    public void Die()
    {
        isDead = true;
        anim.SetBool("isDeath", true);
        moveInput = Vector3.zero;
        StartCoroutine(StartDie());
    }

    private IEnumerator StartDie()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.dayTimeManager.NextDay();
    }

    public void UseStamina()
    {
        playerSaveData.stamina -= workStaminaCost;
        SetGague(staminaBar, playerSaveData.stamina, maxStamina);
        if (playerSaveData.stamina <= 0)
            Die();
    }

    public void SetGague(Image gauge, int value, int maxValue)
    {
        if (value <= 0)
        {
            gauge.fillAmount = 0f;
            return;
        }

        float percent = (float)value / maxValue;

        for (float i = 8f; i >= 1f; i--)
        {
            if (percent >= 0.125f * i)
            {
                gauge.fillAmount = i / 8f;
                break;
            }
        }
    }

    public void CreateDropItem(SelectedItem_UI selectedItem, int count)
    {
        if (selectedItem == null)
        {
            Debug.Log("Player - CreateDropItem 실패");
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = new Vector2(transform.position.x - 0.5f, transform.position.y - 1f);

        Vector2 mouseDir = (mousePos - playerPos).normalized * 2.5f;
        Vector2 bounceBasePos = playerPos + mouseDir;

        GameObject dropItemPrefab = GameManager.Instance.itemManager.GetItemPrefab(selectedItem.GetSelectedItemName());
        GameObject item = Instantiate(dropItemPrefab, bounceBasePos, Quaternion.identity);
        Item _item = item.GetComponent<Item>();

        _item.SpawnItem(true, true, bounceBasePos, selectedItem.selectedSlot.slotItemData, count);
    }

    public void SetHoldItem(ItemData holdItemData = null)
    {
        if (holdItemData == null || holdItemData.IsEmpty() || holdItemData.IsEmpty())
        {
            anim.SetBool("isHoldItem", false);
            holdItem.SetEmpty();
            SetHoldTool();
            return;
        }

        holdItem.gameObject.SetActive(true);

        GameManager.Instance.itemManager.itemDict.TryGetValue(holdItemData.itemName, out Item item);
        if (item.IsEmpty())
            return;
        holdItem.SetHoldItem(item.itemData);

        if (holdItem.itemData.itemType == ItemType.Tool)
        {
            anim.SetBool("isHoldItem", false);
            holdItem.gameObject.SetActive(false);
        }
        else
        {
            anim.SetBool("isHoldItem", true);
        }

        SetHoldTool();
    }

    public void HarvestCrop()
    {
        stateMachine.ChangeState(pickUpState);

        // 현태 선택된 타일에 있는 작물 가져오기
        CropItemData CropItemData = GameManager.Instance.tileManager.GetSelectedCropItemData();

        GameManager.Instance.itemManager.itemDict.TryGetValue(CropItemData.cropName, out Item item);

        playerSaveData.inventory.AddItem(item);

        if (!GameManager.Instance.player.holdItem.IsEmpty())
            return;

        holdItem.gameObject.SetActive(true);
        holdItem.SetHoldItem(item.itemData);
    }

    private void SetHoldTool()
    {
        switch (holdItem.itemData.itemName)
        {
            case "Hoe":
            playerToolType = ToolType.Hoe;
            break;
            case "Pickaxe":
            playerToolType = ToolType.Pickaxe;
            break;
            case "Axe":
            playerToolType = ToolType.Axe;
            break;
            case "WateringCan":
            playerToolType = ToolType.WateringCan;
            break;
            case "Sword":
            playerToolType = ToolType.Sword;
            break;
            default:
            playerToolType = ToolType.None;
            break;
        }
    }

    public void SetPlayerDirection(MouseDirection mouseDirection)
    {
        switch (mouseDirection)
        {
            case MouseDirection.Up:
            playerDir = playerDir.Up;
            anim.SetFloat("vertical", 1f);
            anim.SetFloat("horizontal", 0f);
            break;
            case MouseDirection.Center:
            case MouseDirection.Down:
            playerDir = playerDir.Down;
            anim.SetFloat("vertical", -1f);
            anim.SetFloat("horizontal", 0f);
            break;
            case MouseDirection.Right:
            case MouseDirection.UpRight:
            case MouseDirection.DownRight:
            playerDir = playerDir.Right;
            anim.SetFloat("horizontal", 1f);
            break;
            case MouseDirection.Left:
            case MouseDirection.UpLeft:
            case MouseDirection.DownLeft:
            playerDir = playerDir.Left;
            anim.SetFloat("horizontal", -1f);
            break;
        }
    }

    public bool CanHarvest()
    {
        if (holdItem.IsEmpty() || holdItem.itemData.itemName == GameManager.Instance.tileManager.GetSelectedCropName())
            return true;

        return false;
    }

    public bool CanWork()
    {
        if (holdItem.IsToolHold() && playerSaveData.stamina > 0 && stateMachine.currentState != workingState)
            return true;
        return false;
    }

    public void BuyItem(ItemData buyItemData, int buyCount)
    {
        playerSaveData.gold -= buyItemData.buyPrice * buyCount;
        playerSaveData.inventory.AddItem(buyItemData, buyCount);
    }

    public void SellItem(ItemData sellItemData, int sellCount)
    {
        playerSaveData.gold += sellItemData.sellPrice * sellCount;
        playerSaveData.inventory.RemoveItem(sellItemData, sellCount);
    }

    public void LookDown()
    {
        anim.SetFloat("horizontal", 0f);
        anim.SetFloat("vertical", -1f);
    }

    public void LookUp()
    {
        anim.SetFloat("horizontal", 0f);
        anim.SetFloat("vertical", 1f);
    }

    public void InitializePlayerData()
    {
        SetGague(healthBar, playerSaveData.hp, maxHp);
        SetGague(staminaBar, playerSaveData.stamina, maxStamina);
    }

    public void StartSceneLoad()
    {
        stateMachine.ChangeState(idleState);
        moveInput = Vector3.zero;
    }
}
