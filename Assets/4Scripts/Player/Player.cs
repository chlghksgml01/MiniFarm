using TreeEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;
    private TileManager tileManager;

    [SerializeField] GameObject dropItem;
    PlayerStateMachine stateMachine;

    public Vector3 moveInput;
    public Animator anim;
    public float speed = 5f;

    public PlayerIdleState idleState;
    public PlayerMoveState moveState;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.Log("Player - anim 없음");

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "isMoving");
        moveState = new PlayerMoveState(this, stateMachine, "isMoving");

        stateMachine.Initialize(idleState);

        inventory = new Inventory(GameManager.Instance.uiManager.inventory_UI.slotsUIs.Count);
        tileManager = GameManager.Instance.tileManager;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tileManager != null)
            {
                Vector3Int position = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
                string tileName = tileManager.GetTileName(position);

                if (!string.IsNullOrWhiteSpace(tileName))
                {
                    if(tileName == "InVisible_InteractableTile")
                    {
                        tileManager.SetInteracted(position);
                    }
                }
            }
        }

        stateMachine.currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        transform.Translate(moveInput * speed * Time.fixedDeltaTime);
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
        _item.BounceBasePos = bounceBasePos;
        _item.GetComponent<SpriteRenderer>().sprite = selectedItem.Icon;
        _item.itemData.itemName = selectedItem.itemName;
        _item.itemData.icon = selectedItem.Icon;
        _item.count = count;
        _item.IsBouncing = true;
    }
}
