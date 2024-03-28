using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public class ObjectRenderer(DoomGame game)
{
    public const int TextureSize = 256;
    public const int HalfTextureSize = TextureSize / 2;

    private IDictionary<int, Texture2D> textures;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        textures = Enumerable.Range(1, 5)
            .ToDictionary(i => i, i =>
            {
                var texture = Texture2D.FromFile(graphicsDevice, $"resources/textures/{i}.png");
                return texture;
            });

        foreach (var texture in textures)
        {
            Console.WriteLine($"{texture.Key} = > {texture.Value.Width} x {texture.Value.Height}");
        }
    }

    public void Draw(SpriteBatch spriteBatch, IPlayer player, List<RayCaster.Ray> rays)
    {
        foreach (var ray in rays)
        {
            var texture = textures[ray.WallTexture];
            var rectangle = new Rectangle(ray.Index * RayCaster.Scale, (int)(Screen.HalfHeight - ray.ProjectedHeight / 2f), RayCaster.Scale, (int)ray.ProjectedHeight);
            var sourceRectangle = new Rectangle((int)(ray.WallTextureOffset * texture.Width), 0, RayCaster.Scale, texture.Height);
            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.Lerp(Color.White, Color.Black, ray.Depth / Map.Width));
        }
    }
}