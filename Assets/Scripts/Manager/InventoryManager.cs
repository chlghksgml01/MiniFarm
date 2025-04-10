using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Inventory> inventoryDict = new Dictionary<string, Inventory>();
    public Inventory backpack;

    void Awake()
    {
        backpack = new Inventory(GameManager.Instance.uiManager.GetInventoryUIByName("backpack").slotsUIs.Count);
        inventoryDict.Add("backpack", backpack);
    }

    public Inventory GetInventoryByName(string name)
    {
        if (inventoryDict.ContainsKey(name))
            return inventoryDict[name];

        Debug.Log("InventoryManager - " + name + "Inventory 없음");
        return null;
    }

    public void Add(string name, Item item)
    {
        if (inventoryDict == null)
        {
            Debug.Log("InventoryManager - Inventory Dictionary 비어있음");
            return;
        }

        if (inventoryDict.ContainsKey(name))
            inventoryDict[name].AddItem(item);
        else
        {
            Debug.Log("InventoryManager - " + name + "없음");
            return;
        }

    }
}