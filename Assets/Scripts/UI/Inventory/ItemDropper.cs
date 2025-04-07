using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    static ItemDropper instance;
    public static ItemDropper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ItemDropper");
                instance = obj.AddComponent<ItemDropper>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    public void DropItem(CollectableItem itemPrefab, Vector3 position, Sprite icon, CollectableType type, int quantity)
    {
        var item = Instantiate(itemPrefab, position, Quaternion.identity);
        item.BounceBasePos = position;
        item.GetComponent<SpriteRenderer>().sprite = icon;
        item.Type = type;
        item.Quantity = quantity;
        item.IsBouncing = true;
    }
}
