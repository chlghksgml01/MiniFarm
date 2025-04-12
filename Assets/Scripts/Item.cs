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
            // 인벤 메니저에서 backpack 이름의 인벤토리에 아이템(this) 넣기
            player.inventoryManager.Add("backpack", this);
            GameManager.Instance.uiManager.RefreshInventoryUI("backpack");
            Destroy(gameObject);
        }
    }
}