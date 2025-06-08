using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar_UI : MonoBehaviour
{
    private int slotCount;
    private int selectedSlotIdx = 0;
    private float initialSelectedUIPosX;

    [SerializeField] public List<Slot_UI> slotsUIs;
    [SerializeField] private float nextSelectedUIDistance = 102.5f;
    [SerializeField] private GameObject selectedUI;

    private void Awake()
    {
        slotCount = slotsUIs.Count;
        initialSelectedUIPosX = selectedUI.transform.position.x;
    }

    private void Start()
    {
        if (SceneLoadManager.Instance == null)
        {
            Debug.Log("Inventory_UI - SceneLoadManager ¾øÀ½");
            return;
        }
        DataManager.Instance.SceneLoad += SetToolBarInventory;
    }

    private void OnDisable()
    {
        if (DataManager.Instance == null)
            return;
        DataManager.Instance.SceneLoad -= SetToolBarInventory;
    }

    private void Update()
    {
        float scrollInpt = Input.mouseScrollDelta.y;
        if (scrollInpt != 0)
        {
            selectedSlotIdx += scrollInpt > 0 ? -1 : 1;
            CheckSlot();
            DrawSelectedUI();
        }
    }

    public void CheckSlot()
    {
        if (selectedSlotIdx < 0)
            selectedSlotIdx = slotCount - 1;
        if (selectedSlotIdx >= slotCount)
            selectedSlotIdx = 0;

        ItemData selectedItemData = GameManager.Instance.player.playerSaveData.inventory.slots[selectedSlotIdx].slotItemData;

        if (GameManager.Instance.player.stateMachine.currentState == GameManager.Instance.player.pickUpState)
            return;

        if (!selectedItemData.IsEmpty() && selectedItemData.itemType != ItemType.None)
            GameManager.Instance.player.SetHoldItem(selectedItemData);
        else if (selectedItemData.IsEmpty())
            GameManager.Instance.player.SetHoldItem();
    }

    private void DrawSelectedUI()
    {
        Vector3 nextSelectedUIPos = selectedUI.transform.position;
        nextSelectedUIPos.x = initialSelectedUIPosX + nextSelectedUIDistance * selectedSlotIdx;
        selectedUI.transform.position = nextSelectedUIPos;
    }

    public void SetToolBarInventory()
    {
        Inventory inventory = GameManager.Instance.player.playerSaveData.inventory;

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            if (!inventory.slots[i].IsEmpty())
                slotsUIs[i].SetItem(inventory.slots[i]);
            else
                slotsUIs[i].SetEmpty();
        }
    }

    public void UseItem()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager.player.playerSaveData.inventory.slots[selectedSlotIdx].slotItemData.IsEmpty())
            return;

        gameManager.player.playerSaveData.inventory.slots[selectedSlotIdx].UseItem();
        if (gameManager.player.playerSaveData.inventory.slots[selectedSlotIdx].IsEmpty())
            gameManager.player.SetHoldItem();
        gameManager.uiManager.inventory_UI.Refresh();

        CheckSlot();
    }
}
