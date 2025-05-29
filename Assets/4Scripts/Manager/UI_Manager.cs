using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] public Inventory_UI inventory_UI;
    [SerializeField] public ToolBar_UI toolBar_UI;
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] public GameObject toolBarPanel;
    [SerializeField] public GameObject store;

    private void Start()
    {
        inventory_UI.Refresh();
        inventoryPanel.SetActive(false);
        toolBarPanel.SetActive(true);
        store.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !store.activeSelf)
        {
            ToggleInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (store.activeSelf)
                ToggleStore();
            if (inventoryPanel.activeSelf)
                ToggleInventoryUI();
        }
    }

    public void ToggleInventoryUI()
    {
        if (inventoryPanel == null)
        {
            Debug.Log("UI_Manager - 인벤패널 없음");
            return;
        }

        if (toolBarPanel == null)
        {
            Debug.Log("UI_Manager - 툴바패널 없음");
            return;
        }

        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            toolBarPanel.SetActive(false);
            inventory_UI.Refresh();
        }
        else
        {
            inventoryPanel.SetActive(false);
            toolBarPanel.SetActive(true);

            if (inventory_UI.isDragging)
            {
                inventory_UI.CloseInventoryUI();
            }
        }
    }

    public void ToggleStore()
    {
        if (!store.activeSelf)
        {
            store.SetActive(true);
            inventoryPanel.SetActive(false);
            toolBarPanel.SetActive(false);
        }
        else
        {
            store.SetActive(false);
        }
    }
}