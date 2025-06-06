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
    [SerializeField] public int maxStamina;
    [SerializeField] public int stamina;
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

    bool isDead = false;

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

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "House")
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 1f);
            transform.position = new Vector3(0.5f, 0f, 0f);
        }
        else if (currentSceneName == "Farm")
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", -1f);
            transform.position = new Vector3(0f, 0f, 0f);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.dayTimeManager.OnDayPassed += SetNewDay;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            return;
        GameManager.Instance.dayTimeManager.OnDayPassed -= SetNewDay;
    }

    private void SetNewDay()
    {
        hp = maxHp;
        stamina = maxStamina;

        isDead = false;
        stateMachine.ChangeState(idleState);
        moveInput = Vector3.zero;

        healthBar.fillAmount = 1f;
        staminaBar.fillAmount = 1f;
    }

    void Update()
    {
        if (isDead || GameManager.Instance.uiManager.IsUIOpen())
            return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 단위벡터-> 대각선으로 가도 같은 속도로 이동하게끔
        moveInput = moveInput.normalized;

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
        hp -= _damage;
        SetGague(healthBar, hp, maxHp);
        if (hp <= 0 && !isDead)
        {
            isDead = true;
            GameManager.Instance.dayTimeManager.NextDay();
            anim.SetTrigger("isDeath");
        }
        StartCoroutine(FlashFX());
    }

    public void UseStamina()
    {
        stamina -= workStaminaCost;
        SetGague(staminaBar, stamina, maxStamina);
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
        GameManager.Instance.itemManager.DropItem(dropItemPrefab, bounceBasePos, count);
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
        if (holdItem.IsToolHold() && stamina > 0 && stateMachine.currentState != workingState)
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
}