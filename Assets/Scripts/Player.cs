using UnityEngine;
using UnityEngine.UIElements;
using static Inventory_UI;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public void CreateDropItem(Item itemPrefab, Sprite icon, CollectableType type, int count)
    {
        if(itemPrefab == null || icon == null || type == CollectableType.NONE || count == 0)
        {
            Debug.Log("Player - CreateDropItem ½ÇÆÐ");
            return;
        }

        Vector3 bounceBasePos = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f);

        var item = Instantiate(itemPrefab, bounceBasePos, Quaternion.identity);
        item.BounceBasePos = bounceBasePos;
        item.GetComponent<SpriteRenderer>().sprite = icon;
        item.itemData.type = type;
        item.itemData.count = count;
        item.IsBouncing = true;
    }
}
