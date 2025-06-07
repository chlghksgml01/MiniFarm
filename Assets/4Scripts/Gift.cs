using UnityEngine;

public class Gift : MonoBehaviour
{
    [SerializeField] private Item[] items;

    public void OpenGift()
    {
        for (int i = 0; i < items.Length; i++)
        {
            Item item = Instantiate(items[i]).GetComponent<Item>();
            item.SpawnItem(false, true, transform.position, item.itemData, 1);
            item.SetGiftItem();
        }
        Destroy(gameObject);
    }
}
