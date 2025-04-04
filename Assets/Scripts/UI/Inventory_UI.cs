using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
    [SerializeField] Canvas canvas;

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
        SelectAll,
        SelectOne,
        SelectHalf
    }

    [SerializeField] List<Slot_UI> slotsUI = new List<Slot_UI>();

    public int slotCount = 0;

    GameObject inventoryPanel;
    Player player;
    bool isDragging = false;
    bool isClick = false;

    [SerializeField] SelectedItem selectedItem;
    Slot sourceSlot;

    PointerEventData pointerData;

    private void Awake()
    {
        selectedItem.gameObject.SetActive(false);
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        KeyInput();

        if (isDragging)
            DragItem();
    }

    private void LateUpdate()
    {
        isClick = false;
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (!isClick && !isDragging && Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            GetSlotUI(results, SelectionMode.SelectAll);
        }

        else if (!isClick && !isDragging && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            GetSlotUI(results, SelectionMode.SelectHalf);
        }

        else if (!isClick && !isDragging && Input.GetMouseButtonDown(1))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            GetSlotUI(results, SelectionMode.SelectOne);
        }

    }

    void GetSlotUI(List<RaycastResult> results, SelectionMode selectionMode)
    {
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                if (_slotUI.quantity > 0)
                {
                    isClick = true;
                    isDragging = true;
                    selectedItem.gameObject.SetActive(true);

                    // 선택한 슬롯 저장
                    List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                    sourceSlot = _slots[_slotUI.slotIdx];

                    // SelectedItem 설정
                    selectedItem.type = sourceSlot.type;
                    selectedItem.iconImage.sprite = sourceSlot.icon;
                    switch (selectionMode)
                    {
                        case SelectionMode.SelectAll:
                            selectedItem.textUI.text = sourceSlot.quantity.ToString();
                            sourceSlot.quantity = 0;
                            break;

                        case SelectionMode.SelectOne:
                            selectedItem.textUI.text = "1";
                            sourceSlot.quantity -= 1;

                            break;

                        case SelectionMode.SelectHalf:
                            selectedItem.textUI.text = (sourceSlot.quantity / 2).ToString();
                            sourceSlot.quantity -= int.Parse(selectedItem.textUI.text);
                            break;
                    }

                    if (sourceSlot.quantity == 0)
                        sourceSlot.type = CollectableType.NONE;
                    Refresh();

                    selectedItem.transform.SetParent(transform.root); // UI 최상위로 이동
                    selectedItem.transform.position = pointerData.position;
                }
            }
        }
    }

    void DragItem()
    {
        pointerData.position = Input.mousePosition;
        selectedItem.transform.position = pointerData.position;

        if (!isClick && Input.GetMouseButtonDown(0))
            MoveItem();
    }

    void MoveItem()
    {
        isClick = true;
        List<RaycastResult> results = DetectUIUnderCursor();

        // 슬롯재설정
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                //// 다른 슬롯으로 이동
                //List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                //Slot _slot = _slots[_slotUI.slotIdx];

                //if (_slot.type == CollectableType.NONE)
                //{
                //    _slots[_slotUI.slotIdx].Refresh(selectedSlotType, seletedSlotCount, sourceSlotIcon);
                //    Refresh();
                //    selectedItem.gameObject.SetActive(false);
                //}

                //// 다른 아이템 있었다면 바꾸기
                //// sourceSlot이 selecteSlot으로
                //else
                //{
                //    Slot tempslot = _slot;

                //    Image selectedIcon = selectedItem.transform.Find("Icon").gameObject.GetComponent<Image>();
                //    _slot.icon = selectedIcon.sprite;
                //    _slot.quantity = seletedSlotCount;
                //    _slot.type = selectedSlotType;

                //    selectedIcon.sprite = tempslot.icon;
                //    seletedSlotCount = tempslot.quantity;
                //    selectedSlotType = tempslot.type;
                //    Refresh();
                //}
            }
        }
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
            if (isDragging)
            {
                isDragging = false;
                selectedItem.gameObject.SetActive(false);
            }
        }
    }

    void Refresh()
    {
        if (slotsUI.Count != player.inventory.slots.Count)
        {
            Debug.Log("인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (player.inventory.slots[i].type != CollectableType.NONE)
            {
                slotsUI[i].SetItem(player.inventory.slots[i]);
            }
            else
            {
                slotsUI[i].SetEmtpy();
            }
        }
    }

    List<RaycastResult> DetectUIUnderCursor()
    {
        List<RaycastResult> results;

        // Unity에서 UI 입력을 관리하는 시스템을 가져와서 현재 UI의 입력 이벤트를 처리하려는 변수
        pointerData = new PointerEventData(EventSystem.current);
        // 마우스 커서 위치 저장
        pointerData.position = Input.mousePosition;

        // 레이캐스트 결과 저장용 변수
        results = new List<RaycastResult>();
        // pointerData에 있는 마우스 커서 위치를 기준으로 UI 요소를 검사하고 충돌한 요소를 results에 저장
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    public int GetInventoryCount()
    {
        return slotsUI.Count;
    }
}