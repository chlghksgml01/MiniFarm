using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public event Action<StatType> OnStatChanged;

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

        playerSaveData.inventory = new Inventory(InGameManager.Instance.uiManager.inventory_UI.slotsUIs.Count);
    }
    private void Start()
    {
        sfxManager = SoundManager.Instance.sfxManager;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InGameManager.Instance.dayTimeManager.OnDayFinished += PrepareNewDay;
    }

    private void OnDisable()
    {
        if (InGameManager.Instance == null)
            return;
        InGameManager.Instance.dayTimeManager.OnDayFinished -= PrepareNewDay;
    }

    private void PrepareNewDay()
    {
        playerSaveData.hp = maxHp;

        if (!isDead)
            playerSaveData.stamina = maxStamina;
        else
            playerSaveData.stamina = maxStamina * 3 / 4;

        OnStatChanged?.Invoke(StatType.Health);
        OnStatChanged?.Invoke(StatType.Stamina);

        isDead = false;
        anim.SetBool("isDeath", false);
        stateMachine.ChangeState(idleState);
        moveInput = Vector3.zero;
    }

    void Update()
    {
        if (isDead || InGameManager.Instance.uiManager.IsUIOpen() || SceneLoadManager.Instance.isSceneLoading)
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
        OnStatChanged?.Invoke(StatType.Health);

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
        InGameManager.Instance.dayTimeManager.NextDay();
    }

    public void UseStamina()
    {
        playerSaveData.stamina -= workStaminaCost;
        OnStatChanged?.Invoke(StatType.Stamina);

        if (playerSaveData.stamina <= 0)
            Die();
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

        GameObject dropItemPrefab = InGameManager.Instance.itemManager.GetItemPrefab(selectedItem.GetSelectedItemName());
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

        InGameManager.Instance.itemManager.itemDict.TryGetValue(holdItemData.itemName, out Item item);
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
        CropItemData CropItemData = InGameManager.Instance.tileManager.GetSelectedCropItemData();

        InGameManager.Instance.itemManager.itemDict.TryGetValue(CropItemData.cropName, out Item item);

        playerSaveData.inventory.AddItem(item);
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
        if (holdItem.IsEmpty() || holdItem.itemData.itemName == InGameManager.Instance.tileManager.GetSelectedCropName())
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
        OnStatChanged?.Invoke(StatType.Health);
        OnStatChanged?.Invoke(StatType.Stamina);
    }

    public void StartSceneLoad()
    {
        stateMachine.ChangeState(idleState);
        moveInput = Vector3.zero;
    }
}
