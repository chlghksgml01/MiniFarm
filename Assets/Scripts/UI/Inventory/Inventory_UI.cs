using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
    public string inventoryName;
    private static Inventory_UI instance;
    public static Inventory_UI Instance
    {
        get
        {
            if (instance == null)
            {
                // ������ ���� �����ϱ� ������ ã��
                // ���� ����� ����Ƽ���� �����Ѱ� ����
                instance = FindFirstObjectByType<Inventory_UI>();
                if (instance == null)
                {
                    Debug.LogError("���� Inventory_UI ����");
                }
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


    public GameObject dropItem;
    [SerializeField] SelectedItem_UI selectedItem;

    Inventory inventory;

    // ����������
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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;

        DontDestroyOnLoad(this);

        selectedItem.gameObject.SetActive(false);

        leftClick = new LeftClickStrategy(selectedItem, inventory, dragState);
        rightClick = new RightClickStrategy(selectedItem, inventory, dragState);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem, inventory, dragState);


        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.uiManager.inventoryPanel.SetActive(false);

        for (int i = 0; i < slotsUI.Count; i++)
        {
            slotsUI[i].InitializeSlot(i); // �� ���Կ� �ε��� ����
        }

        slotCount = slotsUI.Count;
    }

    private void Start()
    {
        inventory = GameManager.Instance.player.inventoryManager.GetInventoryByName(inventoryName);
    }

    void Update()
    {
        KeyInput();
        if (dragState.isDragging)
        {
            selectedItem.transform.position = Input.mousePosition;
        }
    }

    private void LateUpdate()
    {
        dragState.isClick = false;
    }

    void KeyInput()
    {
        if (!GameManager.Instance.uiManager.inventoryPanel.activeSelf)
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

    public void Refresh()
    {
        if(inventory == null)
        {
            inventory = GameManager.Instance.player.inventoryManager.GetInventoryByName(inventoryName);
        }

        if (slotsUI.Count != inventory.slots.Count)
        {
            Debug.Log("Inventory_UI - �κ�UI, �κ� ���� �ٸ�");
            return;
        }

        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (inventory.slots[i].type != CollectableType.NONE)
            {
                slotsUI[i].SetItem(inventory.slots[i]);
            }
            else
            {
                slotsUI[i].SetEmtpy();
            }
        }
    }

    public void Remove(int slotId)
    {

    }

    // ��ư �Լ�
    public void SortInventory()
    {
        if (dragState.isDragging)
        {
            PlaceItem();
        }

        inventory.SortInventory();
        Refresh();
    }

    void PlaceItem()
    {
        dragState.isDragging = false;
        bool hasSameItem = false;

        foreach (Slot slot in inventory.slots)
        {
            if (selectedItem.type == slot.type)
            {
                hasSameItem = true;
                slot.count += selectedItem.Count;
                selectedItem.SetEmpty();
            }
        }

        if (!hasSameItem)
        {
            foreach (Slot slot in inventory.slots)
            {
                if (slot.type == CollectableType.NONE)
                {
                    slot.Refresh(selectedItem.type, selectedItem.Icon, selectedItem.Count);
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