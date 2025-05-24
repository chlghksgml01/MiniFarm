using UnityEngine;

public class HoldItem : MonoBehaviour
{
    private SpriteRenderer sprite;
    public bool isSeed { get; set; } = false;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetHoldItem(ItemData itemData)
    {
        sprite.sprite = itemData.icon;
    }
}
