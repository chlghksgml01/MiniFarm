using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ScriptableItemData scriptableItemData;
    public ScriptableCropData scriptableCropData;
    public int count = 1;

    private TextMeshProUGUI textUI;

    private ItemData _itemData;
    public ItemData itemData
    {
        get
        {
            if (scriptableItemData == null)
                return null;

            if (_itemData == null)
            {
                _itemData = new ItemData();

                _itemData.SetItemData(scriptableItemData, scriptableCropData);
                _itemData.count = count;
            }

            return _itemData;
        }
        private set { _itemData = value; }
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

    public bool IsEmpty() => itemData.IsEmpty();

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


    public void SpawnItem(bool _isBouncing, Vector3 bounceBasePos, ItemData _itemData, int _count)
    {
        isBouncing = _isBouncing;
        BounceBasePos = bounceBasePos;

        itemData.itemName = _itemData.itemName;
        itemData.icon = _itemData.icon;
        itemData.itemType = _itemData.itemType;

        GetComponent<SpriteRenderer>().sprite = _itemData.icon;
        count = _count;

        GameManager.Instance.uiManager.toolBar_UI.CheckSlot();
    }
}