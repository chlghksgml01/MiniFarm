using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorePlayerInventory_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> slotsUIs;
    private Inventory playerInventory;

    public void RefreshPlayerInventory()
    {
        if (GameManager.Instance.player == null)
            return;

        if (playerInventory == null)
            InitializePlayerInventory();

        playerInventory = GameManager.Instance.player.playerSaveData.inventory;

        for (int i = 0; i < slotsUIs.Count; i++)
            slotsUIs[i].SetItem(playerInventory.slots[i]);
    }

    public void InitializePlayerInventory()
    {
        playerInventory = new Inventory(GameManager.Instance.uiManager.inventory_UI.slotsUIs.Count);

        int slotCount = playerInventory.slots.Count;
        for (int i = 0; i < slotCount; i++)
        {
            slotsUIs[i].InitializeSlot(i);
            slotsUIs[i].SetItem(playerInventory.slots[i]);
        }
    }

    public void SetSelectedItemData()
    {
        GameObject selectedSlot = EventSystem.current.currentSelectedGameObject;
        Slot_UI slotUI = selectedSlot.GetComponentInChildren<Slot_UI>();
        ItemData itemData = GameManager.Instance.itemManager.GetItemData(slotUI.itemName);

        if (itemData == null)
        {
            Debug.Log("������ ����");
            return;
        }

        itemData.SetItemData(itemData);

        InGameCanvas.Instance.storeUI.isStoreClicked = false;

        int maxCount = int.Parse(selectedSlot.GetComponentInChildren<TextMeshProUGUI>().text);
        InGameCanvas.Instance.storeUI.SetSelectedItemData(itemData, maxCount);
    }
}