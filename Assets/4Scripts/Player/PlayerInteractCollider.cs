
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractCollider : MonoBehaviour
{
    private Player player;
    Coroutine playerDamageCoroutine;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GetItem(collision);

        if (collision.CompareTag("Enemy"))
        {
            Slime slime = collision.GetComponent<Slime>();
            playerDamageCoroutine = StartCoroutine(DamagePlayer(slime));
        }

        if (collision.CompareTag("Gift"))
        {
            GameManager.Instance.giftGet.isGiftGet = true;
            collision.GetComponent<Gift>().OpenGift();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GetItem(collision);
    }

    private void GetItem(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (!item.isPickable)
                return;

            player.playerSaveData.inventory.AddItem(item);
            GameManager.Instance.itemManager.RemoveDropItem(item);
            Destroy(item.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && playerDamageCoroutine != null)
        {
            StopCoroutine(playerDamageCoroutine);
            playerDamageCoroutine = null;
        }
    }

    private IEnumerator DamagePlayer(Slime slime)
    {
        while (true)
        {
            if (slime.slimeState == SlimeState.Death || player.isDead)
            {
                playerDamageCoroutine = null;
                yield break;
            }

            player.Damage(slime.damage);
            yield return new WaitForSeconds(slime.attackDelay);
        }
    }
}