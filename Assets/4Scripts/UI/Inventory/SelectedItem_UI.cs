using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem_UI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI textUI;

    public ItemData selectedItemData = new ItemData();

    private void Awake()
    {
        iconImage.raycastTarget = false;
    }

    public void SetSelectedUIItemData(Inventory.Slot slot, bool isSetCount = false)
    {
        selectedItemData.itemName = slot.slotItemData.itemName;
        selectedItemData.icon = slot.slotItemData.icon;
        selectedItemData.itemType = slot.slotItemData.itemType;

        if (isSetCount)
            selectedItemData.count = slot.slotItemData.count;

        SetCount();

        iconImage.sprite = selectedItemData.icon;
    }

    public void SetCount(int _count = -99)
    {
        if(_count != -99)
            selectedItemData.count = _count;

        if (selectedItemData.count > 1)
            textUI.text = selectedItemData.count.ToString();
        else
            textUI.text = "";
    }

    public void SetEmpty()
    {
        selectedItemData.SetEmpty();

        textUI.text = "";
        iconImage.sprite = null;
        gameObject.SetActive(false);
    }

    public bool IsEmpty()
    {
        if (selectedItemData.IsEmpty())
            return true;
        else
            return false;
    }
}
