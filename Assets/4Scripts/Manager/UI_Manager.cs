using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] public Inventory_UI inventory_UI;
    [SerializeField] public ToolBar_UI toolBar_UI;
    [HideInInspector] public GameObject inventoryPanel;
    [HideInInspector] public GameObject toolBarPanel;
    [SerializeField] public GameObject store;
    [SerializeField] public GameObject option;

    private void Start()
    {
        inventoryPanel = inventory_UI.gameObject;
        toolBarPanel = toolBar_UI.gameObject;

        inventory_UI.Refresh();
        inventoryPanel.SetActive(false);
        toolBarPanel.SetActive(true);
        store.SetActive(false);
        option.SetActive(false);
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
            if (!inventoryPanel.activeSelf && !store.activeSelf)
                ToggleOption();
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
            GameManager.Instance.dayTimeManager.SetTimeStop(true);
            store.SetActive(true);
            inventoryPanel.SetActive(false);
            toolBarPanel.SetActive(false);
        }
        else
        {
            GameManager.Instance.dayTimeManager.SetTimeStop(false);
            store.SetActive(false);
            toolBarPanel.SetActive(true);
        }
    }

    private void ToggleOption()
    {
        if (option.activeSelf)
        {
            option.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            option.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public bool IsUIOpen()
    {
        return inventoryPanel.activeSelf || store.activeSelf;
    }
}