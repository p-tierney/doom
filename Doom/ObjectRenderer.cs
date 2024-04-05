using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public class ObjectRenderer(DoomGame game)
{
    //public const int TextureSize = 256;
    //public const int HalfTextureSize = TextureSize / 2;

    private Texture2D skyTexture;
    private int skyOffset = 0;
    private IDictionary<int, Texture2D> textures;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        skyTexture = Texture2D.FromFile(graphicsDevice, "resources/textures/sky.png");

        textures = Enumerable.Range(1, 5)
            .ToDictionary(i => i, i =>
            {
                var texture = Texture2D.FromFile(graphicsDevice, $"resources/textures/{i}.png");
                return texture;
            });
    }

    public void Draw(SpriteBatch spriteBatch, List<IObject> objects)
    {
        spriteBatch.Draw(skyTexture, new Rectangle(-skyOffset, 0, Screen.Width, Screen.HalfHeight), Color.White);
        spriteBatch.Draw(skyTexture, new Rectangle(-skyOffset + Screen.Width, 0, Screen.Width, Screen.HalfHeight), Color.White);

        foreach (var o in objects.Where(x => x.Depth < Map.Width).OrderByDescending(x => x.Depth))
        {
            spriteBatch.Draw(o.Texture, o.Rectangle, o.TextureRectangle, Color.Lerp(Color.White, Color.Black, o.Depth / Map.Width));
        }
    }

    public interface IObject
    {
        //float X { get; } 
        float Depth { get; }
        //float ProjectedWidth  { get; }
        //float ProjectedHeight  { get; }
        Texture2D Texture  { get; }
        Rectangle Rectangle { get; }
        Rectangle? TextureRectangle { get; }
    }

    internal void Update(DoomGame.Frame frame)
    {
        if (frame.Look.RelativeX != 0)
        {
            var t = skyOffset + frame.Look.RelativeX * 2;
            skyOffset = ((t % Screen.Width) + Screen.Width) % Screen.Width;
        }
    }
}