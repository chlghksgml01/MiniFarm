using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    [SerializeField] Canvas canvas;

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
    [SerializeField] GameObject selectedItemPrefab;

    public int inventorySlotCount = 0;

    GameObject inventoryPanel;
    GameObject selectedItem;
    Player player;
    bool isDragging = false;

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

        inventorySlotCount = slots.Count;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        KeyInput();

        if (isDragging)
            DragItem();
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Unity���� UI �Է��� �����ϴ� �ý����� �����ͼ� ���� UI�� �Է� �̺�Ʈ�� ó���Ϸ��� ����
            pointerData = new PointerEventData(EventSystem.current);
            // ���콺 Ŀ�� ��ġ ����
            pointerData.position = Input.mousePosition;

            // ����ĳ��Ʈ ��� ����� ����
            List<RaycastResult> results = new List<RaycastResult>();
            // pointerData�� �ִ� ���콺 Ŀ�� ��ġ�� �������� UI ��Ҹ� �˻��ϰ� �浹�� ��Ҹ� results�� ����
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                // Ŭ���� UI���� Slot_UI ��������
                Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
                if (_slotUI != null)
                {
                    if (_slotUI.itemCount > 0)
                    {
                        isDragging = true;
                        selectedItem.SetActive(true);

                        // Icon �̹��� ��������
                        GameObject _icon = _slotUI.gameObject.transform.Find("Icon").gameObject;
                        Image iconImage = selectedItem.transform.Find("Icon").gameObject.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = _slotUI.ItemIcon.sprite;
                            iconImage.color = new Color(1, 1, 1, 1);
                        }

                        // Text ��������
                        GameObject _quantity = _slotUI.gameObject.transform.Find("Quantity").gameObject;
                        TextMeshProUGUI quantityText = selectedItem.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
                        if (quantityText != null)
                        {
                            quantityText.text = _slotUI.QuantityText.text;
                        }

                        selectedItem.transform.SetParent(transform.root); // UI �ֻ����� �̵�
                        selectedItem.transform.position = pointerData.position;

                        _slotUI.SetEmtpy();
                    }
                }
            }
        }
    }

    void DragItem()
    {
        if (isDragging)
        {
            pointerData.position = Input.mousePosition;
            selectedItem.transform.position = pointerData.position;
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
            Debug.Log("�κ�UI, �κ� ���� �ٸ�");
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