using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine
{
    public class TextureImage : Asset
    {
        private Texture2D texture;

        public TextureImage(Texture2D texture)
        {
            this.texture = texture;
        }

        protected override void OnDispose()
        {   
            texture.Dispose();
            texture = null;
        }

        public Texture2D GetTexture()
        {
            return texture;
        }

        public int GetWidth()
        {
            return texture.Width;
        }

        public int GetHeight()
        {
            return texture.Height;
        }
    }
}
