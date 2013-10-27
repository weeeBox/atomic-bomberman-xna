using Microsoft.Xna.Framework;

namespace BomberEngine
{
    public class Camera
    {
        private Matrix world;
        private Matrix view;
        private Matrix projection;

        public Camera(Matrix world, Matrix view, Matrix projection)
        {
            this.world = world;
            this.view = view;
            this.projection = projection;
        }

        public Matrix WorldMatrix
        {
            get { return world; }
        }

        public Matrix ViewMatrix
        {
            get { return view; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projection; }
        }
    }
}
