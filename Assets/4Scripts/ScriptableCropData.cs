using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ScriptableData", menuName = "Crop Data")]
public class ScriptableCropData : ScriptableObject
{
    public string cropName;
    public Tile[] cropTiles;
    public Tile[] wetCropTiles;
    public int growthLevel;
    public int[] growthDurations;
    public bool isRegrowable;
    public Sprite regrowImage;
    public Sprite harvestedImage;
}
