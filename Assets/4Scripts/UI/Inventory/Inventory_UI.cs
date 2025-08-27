using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Inventory;

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

    public bool isDragging { get; set; } = false;

    private void Awake()
    {
        leftClick = new LeftClickStrategy(selectedItem);
        rightClick = new RightClickStrategy(selectedItem);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem);

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            slotsUIs[i].InitializeSlot(i); // 각 슬롯에 인덱스 설정
        }
    }

    private void Start()
    {
        if (selectedItem == null)
        {
            Debug.Log("Inventory_UI - selectedItem 없음");
            return;
        }
        selectedItem.gameObject.SetActive(false);
        inventory = InGameManager.Instance.player.playerSaveData.inventory;

        if (SceneLoadManager.Instance == null)
        {
            Debug.Log("Inventory_UI - SceneLoadManager 없음");
            return;
        }
    }

    private void OnEnable()
    {
        if (InGameManager.Instance.player != null)
            GoldUI.text = InGameManager.Instance.player.playerSaveData.gold.ToString();
    }

    void Update()
    {
        KeyInput();
        if (isDragging)
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

        if (slotsUIs.Count != inventory.slots.Count)
        {
            Debug.Log("Inventory_UI - 인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            if (!inventory.slots[i].IsEmpty())
                slotsUIs[i].SetItem(inventory.slots[i]);
            else
                slotsUIs[i].SetEmpty();
        }

        toolBar_UI.SetToolBarInventory();
    }

    // 버튼 함수
    public void SortInventory()
    {
        if (isDragging)
        {
            PlaceItem();
        }

        inventory.SortInventory();
        Refresh();
    }

    void PlaceItem()
    {
        isDragging = false;

        foreach (Slot slot in inventory.slots)
        {
            if (selectedItem.selectedSlot.slotItemData.itemName == slot.slotItemData.itemName)
            {
                slot.itemCount += selectedItem.selectedSlot.itemCount;
                selectedItem.SetEmpty();
                return;
            }
        }

        foreach (Slot slot in inventory.slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetSlotItemData(selectedItem.selectedSlot, 0);
                selectedItem.SetEmpty();
                return;
            }
        }
    }

    public void CloseInventoryUI()
    {
        if (isDragging)
            DropItem();

        isDragging = false;
        selectedItem.gameObject.SetActive(false);
    }

    void DropItem()
    {
        InGameManager.Instance.player.CreateDropItem(selectedItem, selectedItem.selectedSlot.itemCount);
    }

    public void TrashBin()
    {
        selectedItem.SetEmpty();
        isDragging = false;
    }
}