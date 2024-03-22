using Microsoft.Xna.Framework;

namespace Doom;

public class RayCaster(DoomGame game)
{
    public const float FieldOfView = MathF.PI / 3f;
    public const float HalfFieldOfView = FieldOfView / 2f;
    public const int NumberOfRays = Screen.Width / 2;
    public const int HalfNumberOfRays = NumberOfRays / 2;
    public const float DeltaAngle = FieldOfView / NumberOfRays;
    public const int MaxDepth = 20;
    public const float TinyFloat = 0.00001f;

    private List<Ray> CalculateRays(IPlayer player, IMap map)
    {
        var (playerX, playerY) = player.Position;
        var (tileX, tileY) = ((int) playerX, (int) playerY);
        var startAngle = player.Angle - HalfFieldOfView;

        return Enumerable.Range(0, NumberOfRays)
            .Select(i => startAngle + (i * DeltaAngle))
            .Select(rayAngle => 
            {
                var (sin, cos) = MathF.SinCos(rayAngle);
                var distance = MathF.Min(DoHorizontals(), DoVerticals());
                return new Ray(rayAngle, distance);

                float DoHorizontals() 
                {
                    var vX = new Vector2(cos, sin) / MathF.Abs(cos);
                    var tileEdges = Enumerable.Range(0, Map.Width).Select(x => (float) x);
                    foreach (var tileEdge in cos > 0f? tileEdges.Skip(tileX + 1): tileEdges.Take(tileX + 1).Select(x => x - 0.000001f).Reverse())
                    {
                        var dx = MathF.Abs(tileEdge - playerX);
                        var movement = vX * dx;
                        var newPlayerPosition = player.Position + movement;

                        if (map.IsWall(tileEdge, Math.Clamp(newPlayerPosition.Y, 0, Map.Height - 1)))
                        {
                            return movement.Length();
                        }
                    }
                    return float.MaxValue;
                }

                float DoVerticals() 
                {
                    var vY = new Vector2(cos, sin) / MathF.Abs(sin);
                    var tileEdges = Enumerable.Range(0, Map.Height).Select(x => (float) x);
                    foreach (var tileEdge in sin > 0f? tileEdges.Skip(tileY + 1): tileEdges.Take(tileY + 1).Select(x => x - 0.000001f).Reverse())
                    {
                        var dy = Math.Abs(tileEdge - playerY);
                        var movement = vY * dy;
                        var newPlayerPosition = player.Position + movement;

                        if (map.IsWall(Math.Clamp(newPlayerPosition.X, 0, Map.Width - 1), tileEdge))
                        {
                            return movement.Length();
                        }
                    }
                    return float.MaxValue;
                }
            })
            .ToList();
    }

    public void RayCast(IPlayer player, IMap map, ISprites sprites) 
    {
        foreach (var ray in CalculateRays(player, map))
        {
            sprites.DrawLine(player.Position * Screen.TileSize, ray.Distance * Screen.TileSize, ray.Angle, Color.Yellow);
        }

    }

    public record Ray(float Angle, float Distance) 
    {
    }
}