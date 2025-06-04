
using System.Collections;
using UnityEngine;

public class PlayerInteractCollider : MonoBehaviour
{
    private Player player;
    Coroutine playerDamage;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            player.playerSaveData.inventory.AddItem(item);
            Destroy(item.gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            Slime slime = collision.GetComponent<Slime>();
            playerDamage = StartCoroutine(DamagePlayer(slime));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && playerDamage != null)
        {
            StopCoroutine(playerDamage);
            playerDamage = null;
        }
    }

    private IEnumerator DamagePlayer(Slime slime)
    {
        while (true)
        {
            if (slime.slimeState == SlimeState.Death)
                yield return null;

            player.Damage(slime.damage);
            yield return new WaitForSeconds(slime.attackDelay);
        }
    }
}