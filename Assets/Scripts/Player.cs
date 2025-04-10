using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;
    [SerializeField] GameObject dropItem;

    public void CreateDropItem(SelectedItem_UI selectedItem, int count)
    {
        if (dropItem == null || selectedItem == null)
        {
            Debug.Log("Player - CreateDropItem ����");
            return;
        }

        Vector3 bounceBasePos = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f);

        var item = Instantiate(dropItem, bounceBasePos, Quaternion.identity);
        Item _item = item.GetComponent<Item>();
        _item.BounceBasePos = bounceBasePos;
        _item.GetComponent<SpriteRenderer>().sprite = selectedItem.Icon;
        _item.type = selectedItem.type;
        _item.icon = selectedItem.Icon;
        _item.count = count;
        _item.IsBouncing = true;
    }
}
