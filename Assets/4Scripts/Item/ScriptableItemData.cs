using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ScriptableData", menuName = "Item Data")]
public class ScriptableItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int price;

    public DropItemData dropItemData;
    public CropItemData cropItemData;
}
