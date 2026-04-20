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
        TryEnsureUIReferences();
        InitializeUI();
    }

    public void InitializeUI()
    {
        TryEnsureUIReferences();

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        if (toolBarPanel != null)
            toolBarPanel.SetActive(true);
        if (store != null)
            store.SetActive(false);
        if (option != null)
            option.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !IsActive(store))
        {
            ToggleInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsActive(store))
                ToggleStore();
            else if (IsActive(inventoryPanel))
                ToggleInventoryUI();
            else if (!IsActive(inventoryPanel) && !IsActive(store))
                ToggleOption();
        }
    }

    public void ToggleInventoryUI()
    {
        if (inventoryPanel == null)
        {
            Debug.Log("UI_Manager - inventoryPanel 없음");
            return;
        }

        if (toolBarPanel == null)
        {
            Debug.Log("UI_Manager - toolBarPanel 없음");
            return;
        }

        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            toolBarPanel.SetActive(false);
            RefreshInventoryUI();
        }
        else
        {
            inventoryPanel.SetActive(false);
            toolBarPanel.SetActive(true);

            if (inventory_UI.IsDragging)
            {
                inventory_UI.CloseInventoryUI();
            }
        }
    }

    public void ToggleStore()
    {
        if (store == null)
        {
            Debug.Log("UI_Manager - store 없음");
            return;
        }

        if (inventoryPanel == null || toolBarPanel == null)
        {
            TryEnsureUIReferences();
        }

        if (!IsActive(store))
        {
            Time.timeScale = 0f;
            InGameManager.Instance.dayTimeManager.SetTimeStop(true);
            store.SetActive(true);
            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
            if (toolBarPanel != null)
                toolBarPanel.SetActive(false);
        }
        else
        {
            store.GetComponent<StoreUI>().CloseStore();
            Time.timeScale = 1f;
            InGameManager.Instance.dayTimeManager.SetTimeStop(false);
            store.SetActive(false);
            if (toolBarPanel != null)
                toolBarPanel.SetActive(true);
        }
    }

    private void ToggleOption()
    {
        if (option == null)
        {
            Debug.Log("UI_Manager - option 없음");
            return;
        }

        if (IsActive(option))
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
        TryEnsureUIReferences();

        return IsActive(inventoryPanel) || IsActive(store) || IsActive(option);
    }

    public void RefreshInventoryUI()
    {
        if (!TryEnsureUIReferences())
            return;

        if (inventory_UI != null)
            inventory_UI.Refresh();
    }

    public bool TryEnsureUIReferences()
    {
        if (inventory_UI == null)
            inventory_UI = FindFirstObjectByType<Inventory_UI>(FindObjectsInactive.Include);
        if (toolBar_UI == null)
            toolBar_UI = FindFirstObjectByType<ToolBar_UI>(FindObjectsInactive.Include);

        if (inventoryPanel == null && inventory_UI != null)
            inventoryPanel = inventory_UI.gameObject;
        if (toolBarPanel == null && toolBar_UI != null)
            toolBarPanel = toolBar_UI.gameObject;

        return inventory_UI != null && toolBar_UI != null;
    }

    private bool IsActive(GameObject target)
    {
        return target != null && target.activeSelf;
    }
}