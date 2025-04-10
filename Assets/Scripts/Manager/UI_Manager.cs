using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryUIDict = new Dictionary<string, Inventory_UI>();
    public List<Inventory_UI> inventoryUIs;
    public GameObject inventoryPanel;

    private void Awake()
    {
        foreach (Inventory_UI ui in inventoryUIs)
        {
            if (!inventoryUIDict.ContainsKey(ui.inventoryName))
            {
                inventoryUIDict.Add(ui.inventoryName, ui);
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

            if (GetInventoryUIByName("backpack").dragState.isDragging)
            {
                GetInventoryUIByName("backpack").CloseInventoryUI();
            }
        }
    }

    public Inventory_UI GetInventoryUIByName(string _name)
    {
        if (inventoryUIDict.ContainsKey(_name))
        {
            return inventoryUIDict[_name];
        }
        else
        {
            Debug.Log("UI_Manager - Inventory Dictionary에 " + _name + "없음");
            return null;
        }
    }

    public void RefreshInventoryUI(string _name)
    {
        if (inventoryUIDict.ContainsKey(_name))
        {
            inventoryUIDict[_name].Refresh();
        }
        else
        {
            Debug.Log("UI_Manager - Inventory Dictionary에 " + _name + "없음");
            return;
        }
    }
}
