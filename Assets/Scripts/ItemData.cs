using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName = "Item Name";
    public Sprite icon;
}
