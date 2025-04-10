using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite icon = null;
    public CollectableType type;
    public int count = 0;
    TextMeshProUGUI textUI;

    #region DropItemBounce
    bool isBouncing = false;
    Vector3 bounceBasePos;
    float bounceY = 0f;
    float bounceVelocityX = 0.5f;
    float bounceVelocityY = 2f;
    float gravity = 9f;
    float bounceDamping = 0.8f;
    #endregion

    public bool IsBouncing { set { isBouncing = value; } }
    public Vector3 BounceBasePos { get { return bounceBasePos; } set { bounceBasePos = value; } }

    void Awake()
    {
        icon = GetComponent<SpriteRenderer>().sprite;
        textUI = GetComponentInChildren<TextMeshProUGUI>();

        count = 1;
    }

    private void Start()
    {
        bounceVelocityX = Random.Range(0.4f, 0.6f);
        SetTextUI();
    }

    void Update()
    {
        if (isBouncing)
            BounceItem();
    }

    void SetTextUI()
    {
        if (count <= 1)
            textUI.text = "";
        else
            textUI.text = count.ToString();
    }

    void BounceItem()
    {
        // bounceVelocityY : ���ӵ�
        bounceVelocityY -= gravity * Time.deltaTime;
        // ���ӵ��� �ð��� ���� ��ġ �̵�
        bounceY += bounceVelocityY * Time.deltaTime;

        // �ٴڿ� ����������
        if (bounceY <= 0f)
        {
            bounceY = 0f;
            // �� ���� Ƣ�°� �� �۰� Ƣ��
            bounceVelocityY *= -bounceDamping;

            // �۰� Ƣ�� ����
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
            // �κ� �޴������� backpack �̸��� �κ��丮�� ������(this) �ֱ�
            player.inventoryManager.Add("backpack", this);
            GameManager.Instance.uiManager.RefreshInventoryUI("backpack");
            Destroy(gameObject);
        }
    }
}

public enum CollectableType
{
    NONE, CARROT_SEED, CORN_SEED, EGGPLANT_SEED, LETTUCE_SEED, POTATO_SEED, PUMPKIN_SEED, RADISH_SEED, STRAWBERRY_SEED, TOMATO_SEED, WATERMELON_SEED, WHEAT_SEED
}
