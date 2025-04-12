using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;
    [SerializeField] GameObject dropItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.Instance.tileManager.IsInteractable(transform.position))
            {
                Debug.Log("Tile is interactable");
                GameManager.Instance.tileManager.SetInteracted(transform.position);
            }
        }
    }

    public void CreateDropItem(SelectedItem_UI selectedItem, int count)
    {
        if (dropItem == null || selectedItem == null)
        {
            Debug.Log("Player - CreateDropItem ½ÇÆÐ");
            return;
        }

        Vector3 bounceBasePos = new Vector3(transform.position.x + 1.3f, transform.position.y - 1.3f);

        var item = Instantiate(dropItem, bounceBasePos, Quaternion.identity);
        Item _item = item.GetComponent<Item>();
        _item.BounceBasePos = bounceBasePos;
        _item.GetComponent<SpriteRenderer>().sprite = selectedItem.Icon;
        _item.itemData.itemName = selectedItem.itemName;
        _item.itemData.icon = selectedItem.Icon;
        _item.count = count;
        _item.IsBouncing = true;
    }
}
