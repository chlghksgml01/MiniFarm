using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Inventory;

public class Slot_UI : MonoBehaviour
{
    Image itemIcon;
    TextMeshProUGUI quantityText;
    public int slotIdx = 0;

    public Image ItemIcon { get { return itemIcon; } }
    public TextMeshProUGUI QuantityText { get { return quantityText; } }

    public int itemCount = 0;

    public void InitializeSlot(int _slotIdx)
    {
        slotIdx = _slotIdx;
    }

    private void Awake()
    {
        Transform quantityTransform = transform.Find("Quantity");
        if (quantityTransform != null)
        {
            quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.Log("QuantityText 없음");
        }

        Transform image = transform.Find("Icon");
        if (image != null)
        {
            itemIcon = image.GetComponent<Image>();
        }
        else
        {
            Debug.Log("Icon 없음");
        }
    }

    public void SetItem(Inventory.Slot _slot)
    {
        if (_slot != null)
        {
            itemIcon.sprite = _slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            if (_slot.count != 1)
                quantityText.text = _slot.count.ToString();
            else
                quantityText.text = "";

            itemCount = _slot.count;
        }
    }

    public void SetEmtpy()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
        itemCount = 0;
    }
}
