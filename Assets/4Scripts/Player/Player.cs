using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject dropItem;
    [SerializeField] public float speed;

    [HideInInspector] public Inventory inventory;
    private TileManager tileManager;

    [HideInInspector] public Vector3 moveInput;
    [HideInInspector] public Animator anim;

    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerWorkingState workingState;

    public PlayerStateMachine stateMachine { get; private set; }

    [SerializeField] public HoldItem holdItem;
    public bool isHoldItem { get; private set; } = false;
    public ToolType toolType = ToolType.None;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.Log("Player - anim 없음");

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "isMoving");
        moveState = new PlayerMoveState(this, stateMachine, "isMoving");
        workingState = new PlayerWorkingState(this, stateMachine, "isWorking");

        stateMachine.Initialize(idleState);

        inventory = new Inventory(GameManager.Instance.uiManager.inventory_UI.slotsUIs.Count);
        tileManager = GameManager.Instance.tileManager;

        anim.SetFloat("vertical", -1f);
        anim.SetFloat("horizontal", 0f);
    }

    void Update()
    {
        if (stateMachine.currentState != workingState)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // 단위벡터-> 대각선으로 가도 같은 속도로 이동하게끔
            moveInput = moveInput.normalized;

            transform.Translate(moveInput * speed * Time.deltaTime);
        }

        stateMachine.currentState.UpdateState();
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

    public void SetItemHold(bool _isHoldItem, ItemData _holdItem = null)
    {
        isHoldItem = _isHoldItem;
        anim.SetBool("isHoldItem", isHoldItem);

        if (isHoldItem)
        {
            holdItem.gameObject.SetActive(true);
            holdItem.SetHoldItem(_holdItem);

            if (_holdItem.itemType != ItemType.Tool)
                toolType = ToolType.None;
        }
        else
        {
            holdItem.SetHoldSeedNull();
            holdItem.gameObject.SetActive(false);

            if (_holdItem == null && toolType != ToolType.None)
                toolType = ToolType.None;
            else if (_holdItem != null)
                SetToolHold(_holdItem);
        }
    }

    private void SetToolHold(ItemData _holdItem)
    {
        switch (_holdItem.itemName)
        {
            case "Hoe":
                toolType = ToolType.Hoe;
                break;
            case "Pickaxe":
                toolType = ToolType.Pickaxe;
                break;
            case "Axe":
                toolType = ToolType.Axe;
                break;
            case "WateringCan":
                toolType = ToolType.WateringCan;
                break;
            case "FishingRod":
                toolType = ToolType.FishingRod;
                break;
            default:
                toolType = ToolType.None;
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
}