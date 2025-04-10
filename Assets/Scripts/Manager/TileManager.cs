using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] public Tilemap interactableMap;

    [SerializeField] Tile hiddenInteractableTile;
    [SerializeField] Tile interactedTile;

    void Start()
    {
        // Ÿ�ϸ� ���� ��� �� ��ǥ �ϳ��� ��������
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase existingTile = interactableMap.GetTile(position);
            if (existingTile != null)
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(position);
        if (tile == null)
        {
            Debug.Log("TileMananger - position�� Ÿ�� ����");
            return false;
        }

        if (tile.name == "InVisible_InteractableTile")
            return true;
        else
            return false;
    }

    public void SetInteracted(Vector3Int position)
    {
        if(interactedTile == null)
        {
            Debug.Log("TileMananger - interactedTile ����");
            return;
        }    
        interactableMap.SetTile(position, interactedTile);
    }
}
