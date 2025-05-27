using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject dropItem;
    [SerializeField] public float speed;

    [HideInInspector] public Inventory inventory;

    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public Animator anim;

    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerWorkingState workingState;
    [HideInInspector] public PlayerPickUpState pickUpState;

    public PlayerStateMachine stateMachine { get; private set; }

    [SerializeField] public HoldItem holdItem;
    public ToolType playerToolType { get; set; } = ToolType.None;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.Log("Player - anim 없음");

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "isMoving");
        moveState = new PlayerMoveState(this, stateMachine, "isMoving");
        workingState = new PlayerWorkingState(this, stateMachine, "isWorking");
        pickUpState = new PlayerPickUpState(this, stateMachine, "isPickingUp");

        stateMachine.Initialize(idleState);

        inventory = new Inventory(GameManager.Instance.uiManager.inventory_UI.slotsUIs.Count);

        anim.SetFloat("vertical", -1f);
        anim.SetFloat("horizontal", 0f);
    }

    void Update()
    {
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item);
            GameManager.Instance.uiManager.inventory_UI.Refresh();
            Destroy(item.gameObject);
        }
    }

    public void CreateDropItem(SelectedItem_UI selectedItem, int count)
    {
        if (dropItem == null || selectedItem == null)
        {
            Debug.Log("Player - CreateDropItem 실패");
            return;
        }

        Vector3 bounceBasePos = new Vector3(transform.position.x + 1.3f, transform.position.y - 1.3f);

        var item = Instantiate(dropItem, bounceBasePos, Quaternion.identity);
        Item _item = item.GetComponent<Item>();

        _item.SpawnItem(true, bounceBasePos, selectedItem.selectedItemData, count);
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

        var item = GameManager.Instance.itemManager.itemDict[holdItemData.itemName];
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
        CropData cropData = GameManager.Instance.tileManager.GetSelectedCropData();

        GameManager.Instance.itemManager.itemDict.TryGetValue(cropData.cropName, out Item item);

        inventory.AddItem(item);
        GameManager.Instance.uiManager.inventory_UI.Refresh();

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
            case "FishingRod":
                playerToolType = ToolType.FishingRod;
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
                anim.SetFloat("vertical", 1f);
                anim.SetFloat("horizontal", 0f);
                break;
            case MouseDirection.Center:
            case MouseDirection.Down:
                anim.SetFloat("vertical", -1f);
                anim.SetFloat("horizontal", 0f);
                break;
            case MouseDirection.Right:
            case MouseDirection.UpRight:
            case MouseDirection.DownRight:
                anim.SetFloat("horizontal", 1f);
                break;
            case MouseDirection.Left:
            case MouseDirection.UpLeft:
            case MouseDirection.DownLeft:
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
}