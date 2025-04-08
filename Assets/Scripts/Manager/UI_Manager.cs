using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryDict = new Dictionary<string, Inventory_UI>();
    public List<Inventory_UI> inventoryUIs;
    public GameObject inventoryPanel;

    private void Awake()
    {
        foreach (Inventory_UI ui in inventoryUIs)
        {
            if (!inventoryDict.ContainsKey(ui.inventoryName))
            {
                inventoryDict.Add(ui.inventoryName, ui);
            }
        }
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

        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            RefreshInventoryUI("backpack");
        }
        else
        {
            inventoryPanel.SetActive(false);

            // 보류
            //if (dragState.isDragging)
            //{
            //    dragState.isDragging = false;
            //    selectedItem.gameObject.SetActive(false);
            //}
        }
    }

    public void RefreshInventoryUI(string inventoryName)
    {
        if (inventoryDict.ContainsKey(inventoryName))
        {
            inventoryDict[inventoryName].Refresh();
        }
        else
        {
            Debug.Log("UI_Manager - Inventory Dictionary에 " + inventoryName + "없음");
            return;
        }
    }
}
