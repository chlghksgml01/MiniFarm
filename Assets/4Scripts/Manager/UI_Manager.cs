using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] public Inventory_UI inventory_UI;
    [SerializeField] public ToolBar_UI toolBar_UI;
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] public GameObject toolBarPanel;

    private void Start()
    {
        inventory_UI.Refresh();
        inventoryPanel.SetActive(false);
        toolBarPanel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
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
}