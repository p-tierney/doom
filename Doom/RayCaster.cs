using Microsoft.Xna.Framework;

namespace Doom;

public class RayCaster(DoomGame game)
{
    public const float FieldOfView = MathF.PI / 3f;
    public const float HalfFieldOfView = FieldOfView / 2f;
    public const int NumberOfRays = Screen.Width / 1;
    public const int HalfNumberOfRays = NumberOfRays / 2;
    public const float DeltaAngle = FieldOfView / NumberOfRays;
    //public const int MaxDepth = 20;
    public const float TinyFloat = 0.00001f;
    public static readonly float ScreenDistance = Screen.HalfWidth / MathF.Tan(HalfFieldOfView);
    public const int Scale = Screen.Width / NumberOfRays;

    public List<Ray> CalculateRays(IPlayer player, IMap map)
    {
        var (playerX, playerY) = player.Position;
        var playerAngle = player.Angle;
        var (tileX, tileY) = ((int) playerX, (int) playerY);
        var startAngle = playerAngle - HalfFieldOfView;

        return Enumerable.Range(0, NumberOfRays)
            .Select(i => startAngle + (i * DeltaAngle))
            .Select((rayAngle, i) => 
            {
                var (sin, cos) = MathF.SinCos(rayAngle);
                var (h, v) = (DoHorizontals(), DoVerticals());
                var bestCandidate = h.Distance <= v.Distance ? h : v;
                var depth = bestCandidate.Distance * MathF.Cos(playerAngle - rayAngle);
                var projectedHeight = ScreenDistance / depth;
                return new Ray(i, depth, projectedHeight, bestCandidate.Wall!, bestCandidate.WallTextureOffset.GetValueOrDefault());

                RayCandidate DoHorizontals() 
                {
                    if (cos == 0f) return RayCandidate.Max;

                    var vX = new Vector2(cos, sin) / MathF.Abs(cos);
                    var tileEdges = Enumerable.Range(0, Map.Width).Select(x => (float) x);
                    bool lookingRight = cos > 0f;
                    foreach (var tileEdge in lookingRight? tileEdges.Skip(tileX + 1): tileEdges.Take(tileX + 1).Select(x => x - 0.000001f).Reverse())
                    {
                        var dx = MathF.Abs(tileEdge - playerX);
                        var movement = vX * dx;
                        var rayPosition = player.Position + movement;

                        if (map.IsWall(tileEdge, rayPosition.Y, out var location))
                        {
                            //var texture = int.Parse(location.MiniMapChar.ToString());
                            var newY = rayPosition.Y % 1f;
                            var textureOffset = lookingRight ? newY : 1 - newY;

                            //var (s, c) = MathF.SinCos(rayAngle + DeltaAngle);
                            //var d = dx / c;
                            //var y = playerY + d * s;
                            var textureEnd = playerY + MathF.Tan(rayAngle + DeltaAngle) * dx;


                            return new RayCandidate(movement.Length(), location, textureOffset);
                        }
                    }
                    return RayCandidate.Max;
                }

                RayCandidate DoVerticals() 
                {
                    if (sin == 0f) return RayCandidate.Max;

                    var vY = new Vector2(cos, sin) / MathF.Abs(sin);
                    var tileEdges = Enumerable.Range(0, Map.Height).Select(x => (float) x);
                    bool lookingDown = sin > 0f;
                    foreach (var tileEdge in lookingDown? tileEdges.Skip(tileY + 1): tileEdges.Take(tileY + 1).Select(x => x - 0.000001f).Reverse())
                    {
                        var dy = Math.Abs(tileEdge - playerY);
                        var movement = vY * dy;
                        var rayPosition = player.Position + movement;

                        if (map.IsWall(rayPosition.X, tileEdge, out var location))
                        {
                            //var texture = int.Parse(location.MiniMapChar.ToString());
                            var newX = rayPosition.X % 1f;
                            var textureOffset = !lookingDown ? newX : 1 - newX;
                            return new RayCandidate(movement.Length(), location, textureOffset);
                        }
                    }
                    return RayCandidate.Max;
                }
            })
            .ToList();
    }

    /*
    public void RayCast(IPlayer player, IMap map, ISprites sprites) 
    {
        var playerAngle = player.Angle;

        foreach (var ray in CalculateRays(player, map))
        {
            var depth = ray.Distance * MathF.Cos(playerAngle - ray.Angle);
            var projectedHeight = ScreenDistance / depth;
            var rectangle = new Rectangle(ray.Index * Scale, (int)(Screen.HalfHeight - projectedHeight / 2), Scale, (int)projectedHeight);
            sprites.DrawRectangle(rectangle, Color.Lerp(Color.White, Color.Black, depth / Map.Width));
        }
    }
    */

    private record RayCandidate(float Distance, Map.Location? Wall = null, float? WallTextureOffset = null)
    {
        public static readonly RayCandidate Max = new(float.MaxValue);
    }

    public record Ray(int Index, float Depth, float ProjectedHeight, Map.Location Wall, float WallTextureOffset)
    {
    }
}