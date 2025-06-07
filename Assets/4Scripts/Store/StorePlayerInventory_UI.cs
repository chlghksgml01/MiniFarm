using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorePlayerInventory_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> slotsUIs;
    private Inventory playerInventory;

    private void Start()
    {
        playerInventory = GameManager.Instance.player.playerSaveData.inventory;
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
        ItemData itemData = new ItemData();
        itemData.SetItemData(GameManager.Instance.itemManager.GetItemData(slotUI.itemName));

        Store.instance.isStoreClicked = false;
        Store.instance.SetSelectedItemData(itemData);
    }

    public void Refresh()
    {
        if (playerInventory == null)
            return;

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            slotsUIs[i].SetItem(playerInventory.slots[i]);
        }
    }
}