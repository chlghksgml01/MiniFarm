using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ScriptableData", menuName = "Item Data")]
public class ScriptableItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int buyPrice;
    public int sellPrice;

    public EnemyDropItemData dropItemData;
    public CropItemData cropItemData;
}
