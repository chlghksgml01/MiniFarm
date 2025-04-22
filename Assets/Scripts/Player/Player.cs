using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;
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

    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.Instance.tileManager.IsInteractable(transform.position))
            {
                Debug.Log("Tile is interactable");
                GameManager.Instance.tileManager.SetInteracted(transform.position);
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
