using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;
    private void Start()
    {
        inventory = new Inventory(Inventory_UI.Instance.inventorySlotCount);
    }
}
