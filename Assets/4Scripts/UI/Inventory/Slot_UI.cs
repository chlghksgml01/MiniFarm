using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Inventory;

public class Slot_UI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI quantityText;

    public string itemName = "";

    public int slotIdx = 0;
    public int count = 0;

    public void InitializeSlot(int _slotIdx)
    {
        slotIdx = _slotIdx;
    }

    public void SetItem(Slot _slot)
    {
        if (_slot != null)
        {
            if (_slot.IsEmpty())
            {
                SetEmpty();
                return;
            }
            if (_slot.slotItemData.icon == null)
            {
                Debug.Log("slot icon is null");
                return;
            }
            itemIcon.sprite = _slot.slotItemData.icon;
            itemIcon.color = new Color(1, 1, 1, 1);

            if (_slot.itemCount != 1)
                quantityText.text = _slot.itemCount.ToString();
            else
                quantityText.text = "";

            count = _slot.itemCount;
            itemName = _slot.slotItemData.itemName;
        }
    }

    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
        count = 0;
        itemName = "";
    }
}
