
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableData", menuName = "Item Data")]
public class ScriptableItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
}