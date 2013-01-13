using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets.Types;

namespace BomberEngine.Core.Visual
{
    public class Image : VisualElement
    {
        private TextureImage texture;

        public Image(TextureImage texture) : base(texture.GetWidth(), texture.GetHeight())
        {
            this.texture = texture;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            context.DrawImage(texture, drawX, drawY);
            PostDraw(context);
        }
    }
}
