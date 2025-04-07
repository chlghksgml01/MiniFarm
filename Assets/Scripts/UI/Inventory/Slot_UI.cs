using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Inventory;

public class Slot_UI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI quantityText;
    public int slotIdx = 0;

    public Image ItemIcon { get { return itemIcon; } }
    public TextMeshProUGUI QuantityText { get { return quantityText; } }

    public int quantity = 0;

    public void InitializeSlot(int _slotIdx)
    {
        slotIdx = _slotIdx;
    }

    public void SetItem(Slot _slot)
    {
        if (_slot != null)
        {
            if (_slot.quantity == 0)
                return;

            itemIcon.sprite = _slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            if (_slot.quantity != 1)
                quantityText.text = _slot.quantity.ToString();
            else
                quantityText.text = "";

            quantity = _slot.quantity;
        }
    }

    public void SetEmtpy()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
        quantity = 0;
    }
}
