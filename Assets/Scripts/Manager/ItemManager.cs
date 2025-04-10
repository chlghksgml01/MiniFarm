using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item[] items;
    public Dictionary<CollectableType, Item> itemDict = new Dictionary<CollectableType, Item>();

    private void Awake()
    {
        // 현존하는 아이템들 다 넣어놓기
        foreach(Item item in items)
            AddItem(item);
    }

    void AddItem(Item item)
    {
        if(!itemDict.ContainsKey(item.type))
            itemDict.Add(item.type, item);
    }
}