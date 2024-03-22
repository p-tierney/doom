using Microsoft.Xna.Framework;


namespace Doom;

public interface IMap
{
    Map.Location Location(int x, int y);
    Map.Location Location(float x, float y) => Location((int) x, (int) y);
    bool IsWall(int x, int y) => Location(x, y).IsWall;
    bool IsWall(float x, float y) => IsWall((int) x, (int) y);
}

public class Map(List<List<Map.Location>> miniMap): IMap
{
    public const int Width = 16;
    public const int Height = 9;

    public List<Location> NonEmptyLocations = NonEmpty(miniMap).ToList();

    public record Location(int X, int Y, char MiniMapChar) 
    {
        public Rectangle Rectangle = new(X * Screen.TileSize, Y * Screen.TileSize, Screen.TileSize, Screen.TileSize);
        public bool IsWall => MiniMapChar != ' ';
    };

    public const string MiniMap = """
    1111111111111111
    1              1
    1  1111   111  1
    1     1     1  1
    1     1     1  1
    1  1111        1
    1              1
    1   1   1      1
    1111111111111111
    """;

    public static List<List<Location>> ReadMiniMap()
    {
        return MiniMap.Split('\n')
            .Select((line, y) => line
                .Select((c, x) => new Location(x, y, c))
                .ToList()
            )
            .ToList();
    }

    public static IEnumerable<Location> NonEmpty(List<List<Location>> miniMap)
    {
        return ReadMiniMap()
            .SelectMany(x => x)
            .Where(x => x.IsWall)
            .ToList();
    }

    Location IMap.Location(int x, int y) => miniMap[y][x];
}
