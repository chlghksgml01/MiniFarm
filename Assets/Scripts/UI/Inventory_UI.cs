using System.Collections.Generic;
using TMPro;
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

    [SerializeField] List<Slot_UI> slots = new List<Slot_UI>();
    [SerializeField] GameObject selectedItemPrefab;

    public int inventorySlotCount = 0;

    GameObject inventoryPanel;
    GameObject selectedItem;
    Player player;
    bool isDragging = false;
    bool isClick = false;
    List<RaycastResult> results;
    int dragItemIdx = 0;

    PointerEventData pointerData;

    private void Awake()
    {
        selectedItem = Instantiate(selectedItemPrefab);
        selectedItem.transform.SetParent(canvas.transform, false);
        selectedItem.SetActive(false);
        pointerData = new PointerEventData(EventSystem.current);

        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        inventoryPanel = transform.Find("Background").gameObject;
        inventoryPanel.SetActive(false);

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].InitializeSlot(i); // 각 슬롯에 인덱스 설정
        }

        inventorySlotCount = slots.Count;
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
            DetectUIUnderCursor();

            foreach (var result in results)
            {
                // 클릭한 UI에서 Slot_UI 가져오기
                Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
                if (_slotUI != null)
                {
                    if (_slotUI.itemCount > 0)
                    {
                        isClick = true;
                        isDragging = true;
                        selectedItem.SetActive(true);
                        dragItemIdx = _slotUI.slotIdx;

                        // Icon 이미지 가져오기
                        GameObject _icon = _slotUI.gameObject.transform.Find("Icon").gameObject;
                        Image iconImage = selectedItem.transform.Find("Icon").gameObject.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = _slotUI.ItemIcon.sprite;
                            iconImage.color = new Color(1, 1, 1, 1);
                        }

                        // Text 가져오기
                        GameObject _quantity = _slotUI.gameObject.transform.Find("Quantity").gameObject;
                        TextMeshProUGUI quantityText = selectedItem.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
                        if (quantityText != null)
                        {
                            quantityText.text = _slotUI.QuantityText.text;
                        }

                        selectedItem.transform.SetParent(transform.root); // UI 최상위로 이동
                        selectedItem.transform.position = pointerData.position;

                        _slotUI.SetEmtpy();
                    }
                }
            }
        }
    }

    void DragItem()
    {
        pointerData.position = Input.mousePosition;
        selectedItem.transform.position = pointerData.position;

        if (!isClick && Input.GetMouseButtonDown(0))
        {
            isClick = true;
            DetectUIUnderCursor();

            // 슬롯 바꾸기
            foreach (var result in results)
            {
                Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
                if (_slotUI != null)
                {
                    isDragging = false;
                    selectedItem.SetActive(false);

                    List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                    (_slots[dragItemIdx], _slots[_slotUI.slotIdx]) = (_slots[_slotUI.slotIdx], _slots[dragItemIdx]);
                    Refresh();
                }
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
                selectedItem.SetActive(false);
            }
        }
    }

    void Refresh()
    {
        if (slots.Count != player.inventory.slots.Count)
        {
            Debug.Log("인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (player.inventory.slots[i].type != CollectableType.NONE)
            {
                slots[i].SetItem(player.inventory.slots[i]);
            }
            else
            {
                slots[i].SetEmtpy();
            }
        }
    }

    void DetectUIUnderCursor()
    {
        // Unity에서 UI 입력을 관리하는 시스템을 가져와서 현재 UI의 입력 이벤트를 처리하려는 변수
        pointerData = new PointerEventData(EventSystem.current);
        // 마우스 커서 위치 저장
        pointerData.position = Input.mousePosition;

        // 레이캐스트 결과 저장용 변수
        results = new List<RaycastResult>();
        // pointerData에 있는 마우스 커서 위치를 기준으로 UI 요소를 검사하고 충돌한 요소를 results에 저장
        EventSystem.current.RaycastAll(pointerData, results);
    }


    public int GetInventoryCount()
    {
        return slots.Count;
    }
}