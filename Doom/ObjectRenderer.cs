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
        skyTexture = Texture2D.FromFile(graphicsDevice, $"resources/textures/sky.png");

        textures = Enumerable.Range(1, 5)
            .ToDictionary(i => i, i =>
            {
                var texture = Texture2D.FromFile(graphicsDevice, $"resources/textures/{i}.png");
                return texture;
            });

        /*
        foreach (var texture in textures)
        {
            Console.WriteLine($"{texture.Key} = > {texture.Value.Width} x {texture.Value.Height}");
        }
        */
    }

    public void Draw(SpriteBatch spriteBatch, bool spaceDown, IPlayer player, List<RayCaster.Ray> rays)
    {
        spriteBatch.Draw(skyTexture, new Rectangle(-skyOffset, 0, Screen.Width, Screen.HalfHeight), Color.White);
        spriteBatch.Draw(skyTexture, new Rectangle(-skyOffset + Screen.Width, 0, Screen.Width, Screen.HalfHeight), Color.White);

        foreach (var ray in rays)
        {
            var texture = textures[int.Parse(ray.Wall.MiniMapChar.ToString())];
            var rectangle = new Rectangle(ray.Index * RayCaster.Scale, (int)(Screen.HalfHeight - ray.ProjectedHeight / 2f), RayCaster.Scale, (int)ray.ProjectedHeight);

            var startTexture = (int)(ray.WallTextureOffset * texture.Width);
            var endTexture = spaceDown ? (int) (MathF.Sin(RayCaster.DeltaAngle) * ray.Depth * texture.Width) : RayCaster.Scale;
            var sourceRectangle = new Rectangle(startTexture, 0, endTexture, texture.Height);
            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.Lerp(Color.White, Color.Black, ray.Depth / Map.Width));
        }
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