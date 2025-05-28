using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableData", menuName = "Item Data")]
public class ScriptableItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int price;

    public DropItemData dropItemData;
}


[System.Serializable]
public class DropItemData
{
    public int rate = -99;
}
