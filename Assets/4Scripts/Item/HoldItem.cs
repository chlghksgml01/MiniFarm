using UnityEngine;

public class HoldItem : MonoBehaviour
{
    private SpriteRenderer sprite;
    public string cropName { get; private set; }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetHoldItem(ItemData itemData)
    {
        sprite.sprite = itemData.icon;
        if (itemData.itemName.Contains("Seed"))
        {
            string seedName = itemData.itemName;
            cropName = seedName.Replace("Seed", "");
        }
    }

    public void SetHoldSeedNull()
    {
        cropName = "";
    }
}
