using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public Sprite icon;

    private void Awake()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        icon = sprite.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.inventory.AddItem(this);
            Destroy(gameObject);
        }
    }
}

public enum CollectableType
{
    NONE, CARROT_SEED, CORN_SEED, EGGPLANT_SEED, LETTUCE_SEED, POTATO_SEED, PUMPKIN_SEED, RADISH_SEED, STRAWBERRY_SEED, TOMATO_SEED, WATERMELON_SEED, WHEAT_SEED
}
