using UnityEngine;

public class Player : MonoBehaviour
{
    Inventory inventory;
    public Inventory PlayerInventory { get { return inventory; } set { inventory = value; } }
    private void Start()
    {
        inventory = new Inventory(Inventory_UI.Instance.slotCount);
    }
}
