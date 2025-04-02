using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_UI : MonoBehaviour
{
    Image itemIcon;
    TextMeshProUGUI quantityText;

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

    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null)
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            if (slot.count != 1)
                quantityText.text = slot.count.ToString();
        }
    }

    public void SetEmtpy()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
    }
}
