using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public class SpriteObject(DoomGame game, float x, float y, float scale, float heightShift, string resource = "candlebra")
{
    private Vector2 position = new(x, y);
    //private Texture2D texture;
    //private int textureWidth;
    //private int halfTextureWidth;
    //private float imageRatio;

    private TextureList textures;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        textures = new TextureList(graphicsDevice, "resources/sprites/animated_sprites/green_light", 120);
        //texture = Texture2D.FromFile(graphicsDevice, $"resources/sprites/static_sprites/{resource}.png");
        //textureWidth = texture.Width;
        //halfTextureWidth = texture.Width / 2;
        //imageRatio = textureWidth / (float)texture.Height;
    }

    public void Update(DoomGame.Frame frame)
    {

    }

    public class TextureList(GraphicsDevice graphicsDevice, string resourceFolder, int ticksPerFrame)
    {
        private readonly List<Texture2D> textures = Directory.GetFiles(resourceFolder, "*.png")
            .Select(path => Texture2D.FromFile(graphicsDevice, path))
            .ToList();

        public Texture2D GetTexture(GameTime gameTime)
        {
            var ticks = (int)gameTime.TotalGameTime.TotalMilliseconds;
            var frame = ticks / ticksPerFrame;
            return textures[frame % textures.Count];
        }
    }

    public Sprite Get(GameTime gameTime, IPlayer player)
    {
        var texture = textures.GetTexture(gameTime);
        var imageRatio = texture.Width / (float)texture.Height;

        var (dx, dy) = position - player.Position;
        var theta = MathF.Atan2(dy, dx);
        var delta = theta - player.Angle;
        //if ((dx > 0f && player.Angle > MathF.PI) || (dx < 0f && dy < 0f))
        //    delta += MathF.Tau;
        var deltaRays = delta / RayCaster.DeltaAngle;
        var screenX = (RayCaster.HalfNumberOfRays + deltaRays) * RayCaster.Scale;
        var distance = new Vector2(dx, dy).Length();
        var depth = distance * MathF.Cos(delta);
        if (depth < 0.5f) return new Sprite(screenX, float.MaxValue, 0, 0, 0, texture);

        var projectedHeight = RayCaster.ScreenDistance / depth * scale;
        var projectedWidth = projectedHeight * imageRatio;

        return new Sprite(screenX, depth, projectedWidth, projectedHeight, projectedHeight * heightShift, texture);
    }

    public void Draw(SpriteBatch spriteBatch, Sprite sprite) 
    {
        spriteBatch.Draw(sprite.Texture, sprite.Rectangle, Color.White);
    }

    public record Sprite(float X, float Depth, float ProjectedWidth, float ProjectedHeight, float ProjectedHeightShift, Texture2D Texture): ObjectRenderer.IObject
    {
        public Rectangle Rectangle => new((int)(X - (ProjectedWidth / 2f)), (int)(Screen.HalfHeight - (ProjectedHeight / 2f) + ProjectedHeightShift), (int)ProjectedWidth, (int)ProjectedHeight);

        public Rectangle? TextureRectangle => null;
    };
}
