using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DropItemData
{
    public ItemData itemData = new ItemData();
    public Vector3 pos = Vector3.zero;
    public int count = 0;
}

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject dropItemPrefab;
    public Item[] items;
    public Dictionary<string, Item> itemDict = new Dictionary<string, Item>();
    public List<DropItemData> houseDropItems = new List<DropItemData>();
    public List<DropItemData> farmDropItems = new List<DropItemData>();

    private void Awake()
    {
        // 현존하는 아이템들 다 넣어놓기
        foreach (Item item in items)
            AddItem(item);
    }

    private void Start()
    {
        GameManager.Instance.sceneLoadManager.SceneLoad += CreateItem;
    }

    private void OnDisable()
    {
        GameManager.Instance.sceneLoadManager.SceneLoad -= CreateItem;
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
            return item.itemPrefab;
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

    public void DropItem(GameObject dropItem, Vector2 pos, int count)
    {
        DropItemData dropItemData = SetDropItemData(dropItem, pos, count);

        if (SceneManager.GetActiveScene().name == "House")
            houseDropItems.Add(dropItemData);

        else if (SceneManager.GetActiveScene().name == "Farm")
            farmDropItems.Add(dropItemData);
    }

    private DropItemData SetDropItemData(GameObject dropItem, Vector2 pos, int count)
    {
        DropItemData dropItemData = new DropItemData();

        ItemData itemData = dropItem.GetComponent<Item>().itemData;

        dropItemData.itemData.SetItemData(itemData);
        dropItemData.pos = pos;
        dropItemData.count = count;

        return dropItemData;
    }

    public void RemoveDropItem(Item dropItem)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "House")
            RemoveDropItemMethod(houseDropItems, dropItem);
        if (sceneName == "Farm")
            RemoveDropItemMethod(farmDropItems, dropItem);
    }

    private void RemoveDropItemMethod(List<DropItemData> dropItems, Item dropItem)
    {
        foreach (DropItemData dropItemData in dropItems)
        {
            if (dropItemData.pos == dropItem.transform.position
                && dropItemData.itemData.itemName == dropItem.itemData.itemName)
                dropItems.Remove(dropItemData);
        }
    }

    private void CreateItem()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "House")
            CreateItemMethod(houseDropItems);
        else if (sceneName == "Farm")
            CreateItemMethod(farmDropItems);
    }

    private void CreateItemMethod(List<DropItemData> dropItems)
    {
        foreach (DropItemData dropItemData in dropItems)
        {
            GameObject item = Instantiate(dropItemPrefab, dropItemData.pos, Quaternion.identity);
            item.GetComponent<Item>().textUI.text = dropItemData.count.ToString();
            item.GetComponent<SpriteRenderer>().sprite = dropItemData.itemData.icon;
        }
    }
}