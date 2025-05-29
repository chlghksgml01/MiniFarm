using UnityEngine;

public class HoldItem : MonoBehaviour
{
    private SpriteRenderer sprite;
    public ItemData itemData = new ItemData();

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetHoldItem(ItemData newItemData)
    {
        itemData.SetItemData(newItemData);
        sprite.sprite = itemData.icon;
    }

    public bool IsEmpty()
    {
        return itemData.IsEmpty();
    }

    public void SetEmpty()
    {
        if (itemData.IsEmpty())
            return;
        itemData.SetEmpty();
        sprite.sprite = null;
    }

    public bool IsCropSeedHold()
    {
        if (itemData.itemType == ItemType.Seed && !itemData.cropItemData.IsEmpty())
            return true;
        return false;
    }

    public bool IsToolHold()
    {
        if (!itemData.IsEmpty() && itemData.itemType == ItemType.Tool)
            return true;
        return false;
    }
}
