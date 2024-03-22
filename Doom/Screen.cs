namespace Doom;

static class Screen
{
    public const int Width = Map.Width * TileSize;
    public const int HalfWidth = Width / 2;
    public const int Height = Map.Height * TileSize;
    public const int HalfHeight = Height / 2;
    public const int TileSize = 75;
}