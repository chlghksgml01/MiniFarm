using System.Collections.Generic;
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

    private void Start()
    {
        if (SceneLoadManager.Instance == null)
        {
            Debug.Log("ItemManager - SceneLoadManager 없음");
            return;
        }
        SceneLoadManager.Instance.SceneLoad += CreateItem;
    }

    private void OnDisable()
    {
        if (SceneLoadManager.Instance != null)
            SceneLoadManager.Instance.SceneLoad -= CreateItem;
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

    public void DropItem(GameObject _dropItem, Vector2 pos, int count)
    {
        DropItemData dropItemData = SetDropItemData(_dropItem, pos, count);

        if (SceneManager.GetActiveScene().name == "House")
            this.dropItemData.houseDropItems.Add(dropItemData);

        else if (SceneManager.GetActiveScene().name == "Farm")
            this.dropItemData.farmDropItems.Add(dropItemData);
    }

    private DropItemData SetDropItemData(GameObject _dropItem, Vector2 pos, int count)
    {
        DropItemData dropItemData = new DropItemData();

        ItemData itemData = _dropItem.GetComponent<Item>().itemData;

        dropItemData.itemData.SetItemData(itemData);
        dropItemData.pos = pos;
        dropItemData.count = count;

        return dropItemData;
    }

    public void RemoveDropItem(Item _dropItem)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "House")
            RemoveDropItemMethod(dropItemData.houseDropItems, _dropItem);
        if (sceneName == "Farm")
            RemoveDropItemMethod(dropItemData.farmDropItems, _dropItem);
    }

    private void RemoveDropItemMethod(List<DropItemData> dropItems, Item dropItem)
    {
        for (int i = dropItems.Count - 1; i >= 0; i--)
        {
            DropItemData dropItemData = dropItems[i];

            if (dropItemData.pos == dropItem.transform.position &&
                dropItemData.itemData.itemName == dropItem.itemData.itemName)
            {
                dropItems.RemoveAt(i);
            }
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
            if (dropItemData.count > 1)
                item.GetComponent<Item>().textUI.text = dropItemData.count.ToString();
            item.GetComponent<SpriteRenderer>().sprite = dropItemData.itemData.icon;
        }
    }
}