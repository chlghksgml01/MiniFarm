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

    public int count = 0;

    public void InitializeSlot(int _slotIdx)
    {
        slotIdx = _slotIdx;
    }

    public void SetItem(Slot _slot)
    {
        if (_slot != null)
        {
            if (_slot.count == 0)
                return;

            itemIcon.sprite = _slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            if (_slot.count != 1)
                quantityText.text = _slot.count.ToString();
            else
                quantityText.text = "";

            count = _slot.count;
        }
    }

    public void SetEmtpy()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
        count = 0;
    }
}
