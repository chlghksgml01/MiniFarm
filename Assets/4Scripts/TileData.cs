using UnityEngine;

[System.Serializable]
public enum TileState
{
    None,
    Empty,
    Tilled,
    Watered,
    Planted,
}

[System.Serializable]
public enum TileConnectedState
{
    None,
    One,
    Left,
    Right,
    Up,
    Down,
    LeftUp,
    RightUp,
    LeftDown,
    RightDown,
    LeftCenter,
    RightCenter,
    UpCenter,
    DownCenter,
    Center,
    HorizontalCenter,
    VerticalCenter
}

[System.Serializable]
public enum TileConnectedDir
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}

[System.Serializable]
public class TileData
{
    public TileState tileState = TileState.Empty;
    public TileConnectedState tileConnectedState = TileConnectedState.None;
    public TileConnectedDir tileConnectedDir = TileConnectedDir.None;
}

public class CropTileData
{
    public Vector3Int cropPosition = Vector3Int.zero;
    public CropItemData CropItemData = new CropItemData();
}