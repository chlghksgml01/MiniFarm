using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "TilledTileSet", menuName = "TilledTileSet")]
public class TilledTileSet : ScriptableObject
{
    [SerializeField] public TileBase[] tiles;
}
