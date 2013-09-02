using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BomberEngine.Core;
using BomberEngine.Game;
using BomberEngine;
using BomberEngine.Native;

namespace Bomberman
{   
    public class BmGame : Microsoft.Xna.Framework.Game, INativeInterface
    {
        private static readonly int WIDTH = 640;
        private static readonly int HEIGHT = 480;

        #if DEBUG_VIEW
            private static readonly int WIDTH_EX = 640;
        #else
            private static readonly int WIDTH_EX = 0;
        #endif

        private GraphicsDeviceManager graphics;
        private Application application;

        public BmGame(String[] args)
        {
            int width = WIDTH;
            int height = HEIGHT;

            int realWidth = width + WIDTH_EX;
            int realHeight = height;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = realWidth;
            graphics.PreferredBackBufferHeight = realHeight;

            IsFixedTimeStep = true;

            Content.RootDirectory = "Content";

            ApplicationInfo info = new ApplicationInfo(width, height);
            #if WINDOWS
            info.nativeInterface = this;
            info.realWidth = realWidth;
            info.realHeight = realHeight;
            IsMouseVisible = true;
            #endif
            info.args = args;

            application = new BmApplication(Content, info);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Runtime.graphicsDevice = graphics.GraphicsDevice;
            Runtime.contentManager = Content;

            application.Start();

            #if DEBUG && WINDOWS

            int windowX = Application.Storage().GetInt("lastWinX", Int32.MaxValue);
            int windowY = Application.Storage().GetInt("lastWinY", Int32.MaxValue);

            if (windowX != Int32.MaxValue && windowY != Int32.MaxValue)
            {
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                form.Location = new System.Drawing.Point(windowX, windowY);
            }

            #endif
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            RunUpdate(delta);
            
            base.Update(gameTime);
        }

        private void RunUpdate(float delta)
        {   
            if (application.IsRunning())
            {
                application.Update(delta);
            }
            else
            {
                #if DEBUG && WINDOWS
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                Application.Storage().Set("lastWinX", form.Location.X);
                Application.Storage().Set("lastWinY", form.Location.Y);
                Application.Storage().SaveImmediately();
                #endif

                application.RunStop();
                Exit();
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            application.RunStop();
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            application.Draw(graphics.GraphicsDevice);

            base.Draw(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////

        public void SetWindowTitle(string title)
        {
            this.Window.Title = title;
        }
    }
}
