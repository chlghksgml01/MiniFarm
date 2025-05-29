
using UnityEngine;

public class PlayerInteractCollider : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            player.inventory.AddItem(item);
            Destroy(item.gameObject);
        }

        Slime slime = collision.GetComponent<Slime>();
        if (slime != null)
        {
            if (slime.slimeState == SlimeState.Death)
                return;
            player.Damage(slime.damage);
        }
    }
}