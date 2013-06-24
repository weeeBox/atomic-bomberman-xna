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
    public class BombermanGame : Microsoft.Xna.Framework.Game, INativeInterface
    {
        private static readonly int WIDTH = 640;
        private static readonly int HEIGHT = 480;

        private GraphicsDeviceManager graphics;
        private Application application;

        public BombermanGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

            Global.graphicsDevice = graphics.GraphicsDevice;

            Content.RootDirectory = "Content";

            #if WINDOWS
            IsMouseVisible = true;
            #endif

            application = new BombermanApplication(Content, this, WIDTH, HEIGHT);
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
            application.Start();
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
            if (application.IsRunning())
            {
                float delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
                application.Update(delta);
            }
            else
            {
                application.RunStop();
                Exit();
            }
            
            base.Update(gameTime);
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
