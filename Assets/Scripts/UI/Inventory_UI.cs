using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    private static Inventory_UI instance;
    public static Inventory_UI Instance
    {
        get
        {
            if (!instance)
            {
                GameObject go = new GameObject("Inventory_UI");
                instance = go.AddComponent<Inventory_UI>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [SerializeField] List<Slot_UI> slots = new List<Slot_UI>();

    public int inventorySlotCount = 0;

    GameObject inventoryPanel;
    GameObject selectedItem;
    Player player;
    bool isDragging = false;

    PointerEventData pointerData;

    private void Awake()
    {
        selectedItem = new GameObject();
        selectedItem.AddComponent<Image>();

        pointerData = new PointerEventData(EventSystem.current);

        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        inventoryPanel = transform.Find("Background").gameObject;
        inventoryPanel.SetActive(false);

        inventorySlotCount = slots.Count;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        KeyInput();

        if(isDragging && selectedItem != null)
        {
            pointerData.position = Input.mousePosition;
            selectedItem.transform.position = pointerData.position;
        }
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Unity에서 UI 입력을 관리하는 시스템을 가져와서 현재 UI의 입력 이벤트를 처리하려는 변수
            pointerData = new PointerEventData(EventSystem.current);
            // 마우스 커서 위치 저장
            pointerData.position = Input.mousePosition;

            // 레이캐스트 결과 저장용 변수
            List<RaycastResult> results = new List<RaycastResult>();
            // pointerData에 있는 마우스 커서 위치를 기준으로 UI 요소를 검사하고 충돌한 요소를 results에 저장
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                // 클릭한 UI에서 Slot_UI 가져오기
                Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
                if (_slotUI != null)
                {
                    if (_slotUI.itemCount > 0)
                    {
                        isDragging = true;
                        selectedItem.GetComponent<Image>().sprite = _slotUI.ItemIcon.sprite;
                        selectedItem.transform.SetParent(transform.root); // UI 최상위로 이동
                        selectedItem.transform.position = pointerData.position;

                        _slotUI.SetEmtpy();
                    }
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
            inventoryPanel.SetActive(false);
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

    public int GetInventoryCount()
    {
        return slots.Count;
    }
}