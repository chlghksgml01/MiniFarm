using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem_UI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI textUI;

    public Inventory.Slot selectedSlot;

    private void Awake()
    {
        iconImage.raycastTarget = false;
    }

    public void SetSelectedUIItemData(Inventory.Slot slot, int _count = -99)
    {
        selectedSlot.SetSlotItemData(slot, _count);
        SetSelectedItemUI();
    }

    public void SetSelectedItemUI()
    {
        iconImage.sprite = selectedSlot.slotItemData.icon;
        SetCount(selectedSlot.itemCount);
    }

    public void SetCount(int _count = -99)
    {
        if (_count <= 0)
        {
            SetEmpty();
            return;
        }

        if (_count != -99)
            selectedSlot.itemCount = _count;

        if (selectedSlot.itemCount > 1)
            textUI.text = selectedSlot.itemCount.ToString();
        else
            textUI.text = "";
    }

    public void SetEmpty()
    {
        selectedSlot.SetEmpty();

        textUI.text = "";
        iconImage.sprite = null;
        gameObject.SetActive(false);
    }

    public bool IsEmpty()
    {
        if (selectedSlot.IsEmpty())
            return true;
        else
            return false;
    }

    public string GetSelectedItemName()
    {
        if (selectedSlot.IsEmpty())
            return "";
        return selectedSlot.slotItemData.itemName;
    }
}
