
namespace BomberEngine
{
    public class ImageView : View
    {
        private TextureImage texture;

        public ImageView(TextureImage texture)
            : base(texture.GetWidth(), texture.GetHeight())
        {
            this.texture = texture;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            context.DrawImage(texture, 0, 0);
            PostDraw(context);
        }
    }
}
