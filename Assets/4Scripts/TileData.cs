public enum TileState
{
    None,
    Empty,
    Tilled,
    Watered,
    Planted,
    Grown
}

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

public enum TileConnectedDir
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}

public class TileData
{
    public TileState tileState = TileState.Empty;
    public TileConnectedState tileConnectedState = TileConnectedState.None;
    public TileConnectedDir tileConnectedDir = TileConnectedDir.None;
}