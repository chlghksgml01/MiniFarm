using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreInventorySlot : MonoBehaviour
{
    [SerializeField] private ScriptableItemData itemData;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI price;

    private void Awake()
    {
        icon.sprite = itemData.icon;
        price.text = itemData.buyPrice.ToString();
    }

    public void SetSelectedItemData()
    {
        ItemData selectedItemData = new ItemData();
        selectedItemData.SetItemData(itemData);

        Store.instance.isStoreClicked = true;
        Store.instance.SetSelectedItemData(selectedItemData);
    }
}