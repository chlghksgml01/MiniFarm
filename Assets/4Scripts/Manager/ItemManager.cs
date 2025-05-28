using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item[] items;
    public Dictionary<string, Item> itemDict = new Dictionary<string, Item>();

    private void Awake()
    {
        // 현존하는 아이템들 다 넣어놓기
        foreach (Item item in items)
            AddItem(item);
    }

    void AddItem(Item item)
    {
        if (item.IsEmpty())
            return;

        if (!itemDict.ContainsKey(item.itemData.itemName))
            itemDict.Add(item.itemData.itemName, item);
    }

    public GameObject GetItem(string itemName)
    {
        if (itemDict.TryGetValue(itemName, out Item item))
        {
            return item.itemPrefab;
        }
        return null;
    }
}