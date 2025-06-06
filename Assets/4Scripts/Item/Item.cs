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
            Debug.Log("Item - TextMeshProUGUI ����");
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