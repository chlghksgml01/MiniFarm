using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;
    [SerializeField] GameObject dropItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3Int position = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
            if (GameManager.Instance.tileManager.IsInteractable(position))
            {
                Debug.Log("Tile is interactable");
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

        Vector3 bounceBasePos = new Vector3(transform.position.x + 1f, transform.position.y - 1f);

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
