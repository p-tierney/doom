using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Doom;

public interface ISprites
{
    void DrawLine(Vector2 start, float distance, float angle, Color color);
}

public class DoomGame: Game
{
    public const int FramesPerSecond = 60;
    public const float FrameDurationInMillis = 1000f / FramesPerSecond;


    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Texture2D whitePixel;
    private readonly Map map;
    public IMap Map => map;

    public IPlayer Player => player;

    private readonly Player player;
    private readonly RayCaster rayCaster;

    public DoomGame()
    {
        graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Screen.Width,
            PreferredBackBufferHeight = Screen.Height,
        };

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromMicroseconds(FrameDurationInMillis);

        map = new Map(Doom.Map.ReadMiniMap());
        player = new Player(this);
        rayCaster = new RayCaster(this);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        whitePixel = new Texture2D(GraphicsDevice, 1, 1);
        whitePixel.SetData([Color.White]);

        base.LoadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        spriteBatch.Begin();
        foreach (var mapLocation in map.NonEmptyLocations)
        {
            spriteBatch.Draw(whitePixel, mapLocation.Rectangle,  Color.DarkGray);
        }
        spriteBatch.Draw(whitePixel, player.Rectangle,  Color.LightGreen);
        

        rayCaster.RayCast(player, map, new Sprites(spriteBatch, whitePixel));
        //var (x, y) = player.Rectangle.Center;
        //spriteBatch.Draw(whitePixel, new Rectangle(x, y, Screen.Width, 1), null, Color.Yellow, player.Angle, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    private int frameNumber = 0;

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        DoEvents();

        var frame = new Frame(++frameNumber, gameTime, GetPlayerMovement(keyboardState), GetPlayerRotation(keyboardState));
        player.Update(frame);

        base.Update(gameTime);
    }

    private PlayerMovement GetPlayerMovement(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.W)) return PlayerMovement.Forward;
        if (keyboardState.IsKeyDown(Keys.A)) return PlayerMovement.Left;
        if (keyboardState.IsKeyDown(Keys.S)) return PlayerMovement.Backward;
        if (keyboardState.IsKeyDown(Keys.D)) return PlayerMovement.Right;
        return PlayerMovement.None;
    }

    private PlayerRotation GetPlayerRotation(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Right)) return PlayerRotation.Clockwise;
        if (keyboardState.IsKeyDown(Keys.Left)) return PlayerRotation.AntiClockwise;
        return PlayerRotation.None;
    }

    private void DoEvents()
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape)) Exit();
    }

    public class Sprites(SpriteBatch spriteBatch, Texture2D whitePixel) : ISprites
    {
        public void DrawLine(Vector2 start, float distance, float angle, Color color)
        {
            spriteBatch.Draw(whitePixel, new Rectangle((int) start.X, (int) start.Y, (int) distance, 1), null, Color.Yellow, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }

    public record Frame(int Number, GameTime GameTime, PlayerMovement Movement, PlayerRotation Rotation)
    {
        public float ElapsedTimeInMillis => (float)GameTime.ElapsedGameTime.TotalMilliseconds;
    };
}
