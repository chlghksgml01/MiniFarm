using UnityEngine;

public class HoldItem : MonoBehaviour
{
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetHoldItem(ItemData itemData)
    {
        sprite.sprite = itemData.icon;
    }
}
