using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Inventory;

public class DragState
{
    public bool isDragging { get; set; }
}

public class Inventory_UI : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private ToolBar_UI toolBar_UI;
    [SerializeField] private TextMeshProUGUI GoldUI;

    public List<Slot_UI> slotsUIs = new List<Slot_UI>();

    public SelectedItem_UI selectedItem;

    ISelectionStrategy selectionStrategy;
    LeftClickStrategy leftClick;
    RightClickStrategy rightClick;
    ShiftRightClickStrategy shiftRightClick;

    DragState dragState = new DragState();

    public bool IsDragging
    {
        get { return dragState.isDragging; }
        private set { dragState.isDragging = value; }
    }

    private void Awake()
    {
        leftClick = new LeftClickStrategy(selectedItem, dragState);
        rightClick = new RightClickStrategy(selectedItem, dragState);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem, dragState);

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            slotsUIs[i].InitializeSlot(i); // �� ���Կ� �ε��� ����
        }
    }

    private void Start()
    {
        if (selectedItem == null)
        {
            Debug.Log("Inventory_UI - selectedItem ����");
            return;
        }
        selectedItem.gameObject.SetActive(false);
        inventory = InGameManager.Instance.player.playerSaveData.inventory;
    }

    private void OnEnable()
    {
        if (InGameManager.Instance.player != null)
            GoldUI.text = InGameManager.Instance.player.playerSaveData.gold.ToString();
    }

    void Update()
    {
        KeyInput();
        if (IsDragging)
        {
            selectedItem.transform.position = Input.mousePosition;
        }
    }

    void KeyInput()
    {
        if (!InGameManager.Instance.uiManager.inventoryPanel.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            SetStrategy(leftClick);
        }

        else if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            SetStrategy(shiftRightClick);
        }

        else if (Input.GetMouseButtonDown(1))
        {
            SetStrategy(rightClick);
        }
    }

    public void SetStrategy(ISelectionStrategy currentStrategy)
    {
        selectionStrategy = currentStrategy;
        selectionStrategy.ClickHandle();
    }

    public void Refresh()
    {
        inventory = InGameManager.Instance.player.playerSaveData.inventory;

        if (slotsUIs.Count != inventory.GetSlotCount())
        {
            Debug.Log("Inventory_UI - �κ�UI, �κ� ���� �ٸ�");
            return;
        }

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            if (!inventory.IsSlotEmpty(i))
                slotsUIs[i].SetItem(inventory.GetSlot(i));
            else
                slotsUIs[i].SetEmpty();
        }

        toolBar_UI.SetToolBarInventory();
    }

    // ��ư �Լ�
    public void SortInventory()
    {
        if (IsDragging)
        {
            PlaceItem();
        }

        inventory.SortInventory();
        Refresh();
    }

    void PlaceItem()
    {
        IsDragging = false;

        int sameIndex = inventory.HasSameItem(selectedItem.selectedSlot.slotItemData.itemName);

        if (sameIndex != -1)
        {
            int count = inventory.GetSlotItemCount(sameIndex) + selectedItem.selectedSlot.itemCount;
            inventory.SetSlotItemCount(sameIndex, count);
            selectedItem.SetEmpty();
            return;
        }

        else
        {
            if (inventory.IsSlotEmpty(sameIndex))
            {
                inventory.SetSlotItemData(sameIndex, selectedItem.selectedSlot, selectedItem.selectedSlot.itemCount);
                selectedItem.SetEmpty();
                return;
            }
        }
    }

    public void CloseInventoryUI()
    {
        if (IsDragging)
            DropItem();

        IsDragging = false;
        selectedItem.gameObject.SetActive(false);
    }

    void DropItem()
    {
        InGameManager.Instance.player.CreateDropItem(selectedItem, selectedItem.selectedSlot.itemCount);
    }

    public void TrashBin()
    {
        selectedItem.SetEmpty();
        IsDragging = false;
    }
}