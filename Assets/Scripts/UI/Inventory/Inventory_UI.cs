using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
    private static Inventory_UI instance;
    public static Inventory_UI Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Inventory_UI");
                instance = go.AddComponent<Inventory_UI>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    enum SelectionMode
    {
        All,
        Half,
        One,
    }

    [SerializeField] List<Slot_UI> slotsUI = new List<Slot_UI>();

    public int slotCount = 0;

    public GameObject inventoryPanel { get; private set; }
    Player player;
    bool isInventoryAreaClicked = false;

    public GameObject dropItem;
    [SerializeField] SelectedItem_UI selectedItem;

    PointerEventData pointerData;

    // 디자인패턴
    ISelectionStrategy selectionStrategy;
    private LeftClickStrategy leftClick;
    private RightClickStrategy rightClick;
    private ShiftRightClickStrategy shiftRightClick;


    public class DragState
    {
        public bool isClick = false;
        public bool isDragging = false;
    }
    DragState dragState = new DragState();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        selectedItem.gameObject.SetActive(false);

        leftClick = new LeftClickStrategy(selectedItem, player, dragState);
        rightClick = new RightClickStrategy(selectedItem, player, dragState);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem, player, dragState);

        pointerData = new PointerEventData(EventSystem.current);

        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        inventoryPanel = transform.Find("Background").gameObject;
        inventoryPanel.SetActive(false);

        for (int i = 0; i < slotsUI.Count; i++)
        {
            slotsUI[i].InitializeSlot(i); // 각 슬롯에 인덱스 설정
        }

        slotCount = slotsUI.Count;
    }

    void Update()
    {
        KeyInput();
        if (dragState.isDragging)
        {
            pointerData = new PointerEventData(EventSystem.current);
            selectedItem.transform.position = Input.mousePosition;
        }
    }

    private void LateUpdate()
    {
        dragState.isClick = false;
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (!inventoryPanel.activeSelf)
            return;

        if (!dragState.isClick && Input.GetMouseButtonDown(0))
        {
            SetStrategy(leftClick);
            selectionStrategy.ClickHandle();
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            SetStrategy(shiftRightClick);
            selectionStrategy.ClickHandle();
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1))
        {
            SetStrategy(rightClick);
            selectionStrategy.ClickHandle();
        }
    }
    public void SetStrategy(ISelectionStrategy currentStrategy)
    {
        selectionStrategy = currentStrategy;
    }

    public void ToggleInventory()
    {
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Refresh();
        }
        else
        {
            inventoryPanel.SetActive(false);
            if (dragState.isDragging)
            {
                dragState.isDragging = false;
                selectedItem.gameObject.SetActive(false);
            }
        }
    }

    public void Refresh()
    {
        if (slotsUI.Count != player.PlayerInventory.slots.Count)
        {
            Debug.Log("인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (player.PlayerInventory.slots[i].type != CollectableType.NONE)
            {
                slotsUI[i].SetItem(player.PlayerInventory.slots[i]);
            }
            else
            {
                slotsUI[i].SetEmtpy();
            }
        }
    }

    // 버튼 함수
    public void SortInventory()
    {
        if (dragState.isDragging)
        {
            PlaceItem();
        }

        player.PlayerInventory.SortInventory();
        Refresh();
    }

    void PlaceItem()
    {
        dragState.isDragging = false;
        bool hasSameItem = false;

        foreach (Slot slot in player.PlayerInventory.slots)
        {
            if (selectedItem.type == slot.type)
            {
                hasSameItem = true;
                slot.quantity += selectedItem.Quantity;
                selectedItem.SetEmpty();
            }
        }

        if (!hasSameItem)
        {
            foreach (Slot slot in player.PlayerInventory.slots)
            {
                if (slot.type == CollectableType.NONE)
                {
                    slot.Refresh(selectedItem.type, selectedItem.Icon, selectedItem.Quantity);
                    selectedItem.SetEmpty();
                }
            }
        }
    }

    public void TrashBin()
    {
        selectedItem.SetEmpty();
        dragState.isDragging = false;
    }
}