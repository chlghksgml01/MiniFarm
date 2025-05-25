using UnityEngine;

public class HoldItem : MonoBehaviour
{
    private SpriteRenderer sprite;
    public ItemData itemData = new ItemData();

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetHoldItem(ScriptableItemData scriptableItemData, ScriptableCropData scriptableCropData)
    {
        itemData.SetItemData(scriptableItemData, scriptableCropData);
        sprite.sprite = itemData.icon;
    }

    public void SetEmpty()
    {
        itemData.SetEmpty();
        sprite.sprite = null;
    }

    public bool IsCropSeedHold()
    {
        if (itemData.itemType == ItemType.Seed && !itemData.cropData.IsEmpty())
            return true;
        return false;
    }
}
