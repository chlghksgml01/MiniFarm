using TMPro;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] CollectableType type;
    SpriteRenderer sprite;
    TextMeshProUGUI textUI;
    Sprite icon;
    int quantity = 1;

    #region DropItemBounce
    bool isBouncing = false;
    Vector3 bounceBasePos;
    float bounceY = 0f;
    float bounceVelocityX = 0.5f;
    float bounceVelocityY = 2f;
    float gravity = 9f;
    float bounceDamping = 0.8f;
    #endregion

    public int Quantity { get { return quantity; } set { quantity = value; } }
    public bool IsBouncing { set { isBouncing = value; } }
    public Vector3 BounceBasePos { get { return bounceBasePos; } set { bounceBasePos = value; } }

    public CollectableType Type
    {
        get { return type; }
        set { type = value; }
    }
    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        textUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        bounceVelocityX = Random.Range(0.4f, 0.6f);
        icon = sprite.sprite;

        SetTextUI();
    }

    void Update()
    {
        if (isBouncing)
            BounceItem();
    }

    void SetTextUI()
    {
        if (quantity <= 1)
            textUI.text = "";
        else
            textUI.text = quantity.ToString();
    }

    void BounceItem()
    {
        // bounceVelocityY : 가속도
        bounceVelocityY -= gravity * Time.deltaTime;
        // 가속도로 시간에 따라 위치 이동
        bounceY += bounceVelocityY * Time.deltaTime;

        // 바닥에 떨어졌으면
        if (bounceY <= 0f)
        {
            bounceY = 0f;
            // 그 다음 튀는건 더 작게 튀기
            bounceVelocityY *= -bounceDamping;

            // 작게 튀면 정지
            if (Mathf.Abs(bounceVelocityY) < 1f)
            {
                bounceVelocityY = 0f;
                isBouncing = false;
            }
        }

        bounceBasePos.x += bounceVelocityX * Time.deltaTime;
        transform.position = bounceBasePos + new Vector3(0, bounceY, 0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.PlayerInventory.AddItem(this);
            Destroy(gameObject);
            Inventory_UI.Instance.Refresh();
        }
    }
}

public enum CollectableType
{
    NONE, CARROT_SEED, CORN_SEED, EGGPLANT_SEED, LETTUCE_SEED, POTATO_SEED, PUMPKIN_SEED, RADISH_SEED, STRAWBERRY_SEED, TOMATO_SEED, WATERMELON_SEED, WHEAT_SEED
}
