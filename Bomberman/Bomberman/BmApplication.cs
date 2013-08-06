using BomberEngine.Core.Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Native;
using Bomberman.Game;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;
using BomberEngine.Core.Visual;

namespace Bomberman
{
    public class BmApplication : Application
    {
        private ContentManager contentManager;

        public BmApplication(ContentManager contentManager, ApplicationInfo info)
            : base(info)
        {
            this.contentManager = contentManager;
        }

        protected override void OnStart()
        {
            SpriteFont systemFont = contentManager.Load<SpriteFont>("SystemFont");
            Helper.fontSystem = new VectorFont(systemFont);
            Helper.fontButton = new VectorFont(contentManager.Load<SpriteFont>("ButtonFont"));
            Helper.fontFPS = new VectorFont(contentManager.Load<SpriteFont>("FPSFont"));

            context.SetSystemFont(systemFont);

            InitFPS();
        }

        protected override AssetManager CreateAssetManager()
        {
            return new BmAssetManager(contentManager);
        }

        protected override RootController CreateRootController()
        {
            return new BmRootController(contentManager);
        }

        public static new BmAssetManager Assets()
        {
            return Application.Assets() as BmAssetManager;
        }

        public static new BmRootController RootController()
        {
            return Application.RootController() as BmRootController;
        }

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnDrawDebug(Context context)
        {
            if (CVars.g_drawFPS.boolValue)
            {
                m_fpsDrawable.NextFrame();
                m_fpsDrawable.Draw(context);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region FPS

        private FPSDrawable m_fpsDrawable;

        [System.Diagnostics.Conditional("DEBUG")]
        private void InitFPS()
        {
            m_fpsDrawable = new FPSDrawable(Helper.fontFPS, 0.2f);
        }

        #endregion
    }
}
