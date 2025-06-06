using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData _itemData;
    public ScriptableItemData scriptableItemData;

    public TextMeshProUGUI textUI;
    private bool isPlayerDrop;

    public ItemData itemData
    {
        get
        {
            if (scriptableItemData == null)
                return null;

            if (_itemData == null)
            {
                _itemData = new ItemData();

                _itemData.SetItemData(scriptableItemData);
            }

            return _itemData;
        }
        set { _itemData = value; }
    }
    #region DropItemBounce
    bool isBouncing = false;
    Vector3 bounceBasePos;
    float bounceY = 0f;
    float bounceVelocityX = 0.5f;
    float bounceVelocityY = 3f;
    float gravity = 9f;
    float bounceDamping = 0.7f;
    #endregion

    public Vector3 BounceBasePos { get { return bounceBasePos; } set { bounceBasePos = value; } }

    void Awake()
    {
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        if (textUI == null)
            Debug.Log("Item - TextMeshProUGUI 없음");
    }

    private void Start()
    {
        bounceVelocityX = Random.Range(0.4f, 0.6f);
    }

    void Update()
    {
        if (isBouncing)
            BounceItem();
    }

    public bool IsEmpty() => itemData.IsEmpty();

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

        float dirX = 1f;
        if (isPlayerDrop)
            dirX = (transform.position - GameManager.Instance.player.transform.position).normalized.x;
        bounceBasePos.x += dirX * bounceVelocityX * Time.deltaTime;
        transform.position = bounceBasePos + new Vector3(0, bounceY, 0);
    }

    public void SpawnItem(bool _isPlayerDrop, bool _isBouncing, Vector3 bounceBasePos, ItemData _itemData, int _count)
    {
        isPlayerDrop = _isPlayerDrop;
        isBouncing = _isBouncing;
        BounceBasePos = bounceBasePos;

        itemData.itemName = _itemData.itemName;
        itemData.icon = _itemData.icon;
        itemData.itemType = _itemData.itemType;
        SetCount(_count);

        GetComponent<SpriteRenderer>().sprite = _itemData.icon;

        GameManager.Instance.uiManager.toolBar_UI.CheckSlot();
    }

    private void SetCount(int count)
    {
        if (count > 1)
            textUI.text = count.ToString();
        else
            textUI.text = "";
    }
}