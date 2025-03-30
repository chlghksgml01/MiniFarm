using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player)
        {
            player.inventory.AddItem(type);
            Destroy(gameObject);
        }
    }
}

public enum CollectableType
{
    NONE, CARROT_SEED
}
