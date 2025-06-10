using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public Item[] items;
    public Dictionary<string, Item> itemDict = new Dictionary<string, Item>();
    public DropItemDatas dropItemData { get; set; } = new DropItemDatas();

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

    public GameObject GetItemPrefab(string itemName)
    {
        if (itemDict.TryGetValue(itemName, out Item item))
        {
            return item.itemData.itemPrefab;
        }
        return null;
    }

    public ItemData GetItemData(string itemName)
    {
        if (itemDict.TryGetValue(itemName, out Item item))
        {
            return item.itemData;
        }
        return null;
    }

    public void SaveDropItem()
    {
        if (SceneManager.GetActiveScene().name == "House")
        {
            dropItemData.houseDropItems.Clear();
            SetDropItemData(dropItemData.houseDropItems);
        }

        else if (SceneManager.GetActiveScene().name == "Farm")
        {
            dropItemData.farmDropItems.Clear();
            SetDropItemData(dropItemData.farmDropItems);
        }
    }

    private void SetDropItemData(List<DropItemData> dropItemsLocation)
    {
        GameObject[] itemGameObjects = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject itemGameObject in itemGameObjects)
        {
            ItemData itemData = new ItemData();
            itemData.SetItemData(itemGameObject.GetComponent<Item>().itemData);

            DropItemData _dropItemData = new DropItemData();
            _dropItemData.itemData = itemData;
            _dropItemData.pos = itemGameObject.transform.position;

            if (itemGameObject.GetComponentInChildren<TextMeshProUGUI>().text == "")
                _dropItemData.count = 1;
            else
                _dropItemData.count = int.Parse(itemGameObject.GetComponentInChildren<TextMeshProUGUI>().text);

            dropItemsLocation.Add(_dropItemData);
        }
    }

    public void CreateItem()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "House")
            CreateItemMethod(dropItemData.houseDropItems);
        else if (sceneName == "Farm")
            CreateItemMethod(dropItemData.farmDropItems);
    }

    private void CreateItemMethod(List<DropItemData> dropItems)
    {
        foreach (DropItemData dropItemData in dropItems)
        {
            GameObject itemPrefab = GetItemPrefab(dropItemData.itemData.itemName);
            GameObject item = Instantiate(itemPrefab, dropItemData.pos, Quaternion.identity);

            item.GetComponent<Item>().InitializeItem(dropItemData);
        }
    }
}