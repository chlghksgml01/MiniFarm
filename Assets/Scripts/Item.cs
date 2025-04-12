using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public int count = 1;
    TextMeshProUGUI textUI;

    #region DropItemBounce
    bool isBouncing = false;
    Vector3 bounceBasePos;
    float bounceY = 0f;
    float bounceVelocityX = 0.5f;
    float bounceVelocityY = 3f;
    float gravity = 9f;
    float bounceDamping = 0.7f;
    #endregion

    public bool IsBouncing { set { isBouncing = value; } }
    public Vector3 BounceBasePos { get { return bounceBasePos; } set { bounceBasePos = value; } }

    void Awake()
    {
        textUI = GetComponentInChildren<TextMeshProUGUI>();
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
            if (Mathf.Abs(bounceVelocityY) < 1.5f)
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