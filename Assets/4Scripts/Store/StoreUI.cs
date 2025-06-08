using TMPro;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField] public TMP_InputField itemCountTextUI;
    [SerializeField] public TextMeshProUGUI playerGold;
    [SerializeField] private StorePlayerInventory_UI storePlayerInventory_UI;

    public bool isStoreClicked { get; set; } = false;
    private bool prevIsStoreClicked = false;

    [HideInInspector] public ItemData selectedItemData = new ItemData();
    private bool hasBeenSelected = false;
    private Player player;

    private void Start()
    {
        if (player == null)
        {
            player = GameManager.Instance.player;
        }
        playerGold.text = player.playerSaveData.gold.ToString();

        itemCountTextUI.text = "";
    }

    private void OnEnable()
    {
        storePlayerInventory_UI.RefreshPlayerInventory();
    }

    public void Purchase()
    {
        int selectedItemCount = int.Parse(itemCountTextUI.text);
        if (isStoreClicked)
        {
            if (selectedItemData.buyPrice * selectedItemCount <= player.playerSaveData.gold)
            {
                player.BuyItem(selectedItemData, selectedItemCount);
                storePlayerInventory_UI.RefreshPlayerInventory();
                playerGold.text = player.playerSaveData.gold.ToString();
            }
            else
            {
                Debug.Log("Store - 구매 실패: 골드 부족");
            }
        }

        else
        {
            player.SellItem(selectedItemData, selectedItemCount);
            storePlayerInventory_UI.RefreshPlayerInventory();
            playerGold.text = player.playerSaveData.gold.ToString();
        }

        itemCountTextUI.text = "";
    }

    public void SetSelectedItemData(ItemData itemData)
    {
        if (itemData == null || itemData.IsEmpty())
        {
            SetSelectedData("", false, null);
            return;
        }

        // 같은 아이템 선택했을 경우
        if (hasBeenSelected && selectedItemData.itemName == itemData.itemName && prevIsStoreClicked == isStoreClicked)
        {
            int itemCount = int.Parse(itemCountTextUI.text) + 1;
            SetSelectedData(itemCount.ToString(), true, itemData);
            return;
        }

        // 다른 아이템 선택했을 경우
        else if (hasBeenSelected || selectedItemData.itemName != itemData.itemName || prevIsStoreClicked != isStoreClicked)
        {
            SetSelectedData("1", _hasBeenSelected: true, _selectedItemData: itemData);
            return;
        }
    }

    private void SetSelectedData(string text, bool _hasBeenSelected, ItemData _selectedItemData)
    {
        itemCountTextUI.text = text;
        hasBeenSelected = _hasBeenSelected;
        if (_selectedItemData == null)
            selectedItemData.SetEmpty();
        else
            selectedItemData = _selectedItemData;

        prevIsStoreClicked = isStoreClicked;
    }
}
