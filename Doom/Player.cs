using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Doom;

public interface IPlayer
{
    Vector2 Position { get; }
    float Angle { get; }
}

public class Player(DoomGame game, float x = Player.StartX, float y = Player.StartY, float angle = Player.StartAngle): IPlayer
{
    public const float StartX = 1.5f;
    public const float StartY = 5f;
    public const float StartAngle = 0f;
    public const float Speed = 0.004f;
    public const float RotationSpeed = 0.002f;
    public const float Size = 0.25f;
    public const float HalfSize = Size / 2f;

    public Vector2 Position => new(x, y);
    public float Angle => angle;
    public Rectangle Rectangle => new((int)(x * Screen.TileSize) - 10, (int)(y * Screen.TileSize) - 10, 20, 20);

    public (int X, int Y) MapPosition = ((int)x, (int) y);

    private void SetAngle(float value) => angle = value %= MathF.Tau;

    private static readonly List<Vector2> Corners = [
        new(-HalfSize, -HalfSize), // top-left
        new(HalfSize, -HalfSize), // top-right
        new(HalfSize, HalfSize), // bottom-right
        new(-HalfSize, HalfSize), // bottom-left
    ];

    private bool IsWall(Vector2 centre)
    {
        return Corners.Select(corner => centre + corner).Any(corner => game.Map.IsWall(corner.X, corner.Y));
    }

    private void SetPosition(float dx, float dy)
    {
        if (!IsWall(new(x + dx, y))) x += dx;
        if (!IsWall(new(x, y + dy))) y += dy;
    }

    public void DoMovement(DoomGame.Frame frame)
    {
        var (sin, cos) = MathF.SinCos(angle);
        var speed = Player.Speed * frame.ElapsedTimeInMillis;
        var (speedSin, speedCos) = (sin * speed, cos * speed);

        var (dx, dy) = CalculateChangeInPosition();
        SetPosition(dx, dy);

        SetAngle(CalculateNewAngle());

        (float, float) CalculateChangeInPosition()
        {
            return frame.Movement switch
            {
                PlayerMovement.Left => (speedSin, -speedCos),
                PlayerMovement.Right => (-speedSin, speedCos),
                PlayerMovement.Forward => (speedCos, speedSin),
                PlayerMovement.Backward => (-speedCos, -speedSin),
                _ => (0f, 0f),
            };
        }

        float CalculateNewAngle() 
        {
            return frame.Rotation switch
            {
                PlayerRotation.Clockwise => angle += Player.RotationSpeed * frame.ElapsedTimeInMillis,
                PlayerRotation.AntiClockwise => angle -= Player.RotationSpeed * frame.ElapsedTimeInMillis,
                _ => angle,
            };
        }
    }

    public void Update(DoomGame.Frame frame)
    {
        DoMovement(frame);
    }
}

public enum PlayerMovement
{
    None, Left, Right, Forward, Backward
}

public enum PlayerRotation
{
    None, Clockwise, AntiClockwise
}
