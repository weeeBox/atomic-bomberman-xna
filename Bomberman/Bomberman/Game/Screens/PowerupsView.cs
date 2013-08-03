using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BomberEngine.Core;
using BomberEngine.Core.Assets.Types;
using Assets;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class PowerupsView : View
    {   
        private TextureImage[] powerupImages;
        private Field field;

        private Color innactiveColor;

        public PowerupsView(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;
            innactiveColor = new Color(0, 0, 0, 0.5f);

            InitPowerupImages();
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            int drawX = 0;
            int drawY = 0;

            Player player = field.GetPlayers().list[0];
            int[] powerups = player.powerups.powerups;

            int powerup = 0;
            foreach (int count in powerups)
            {
                TextureImage image = powerupImages[powerup];
                drawY = 0;

                context.DrawImage(image, drawX, drawY);

                for (int i = 1; i < count; ++i)
                {
                    context.DrawImage(image, drawX, drawY);
                    drawX += 2;
                    drawY += 2;
                }

                if (count == 0)
                {
                    context.FillRect(drawX, drawY, image.GetWidth(), image.GetHeight(), innactiveColor);
                }

                drawX += image.GetWidth();
                ++powerup;
            }

            PostDraw(context);
        }

        private void InitPowerupImages()
        {
            powerupImages = new TextureImage[]
            {
                Helper.GetTexture(A.gfx_powerups_bomb),
                Helper.GetTexture(A.gfx_powerups_flame),
                Helper.GetTexture(A.gfx_powerups_disea),
                Helper.GetTexture(A.gfx_powerups_kick),
                Helper.GetTexture(A.gfx_powerups_skate),
                Helper.GetTexture(A.gfx_powerups_punch),
                Helper.GetTexture(A.gfx_powerups_grab),
                Helper.GetTexture(A.gfx_powerups_spooge),
                Helper.GetTexture(A.gfx_powerups_gold),
                Helper.GetTexture(A.gfx_powerups_trig),
                Helper.GetTexture(A.gfx_powerups_jelly),
                Helper.GetTexture(A.gfx_powerups_ebola),
                Helper.GetTexture(A.gfx_powerups_random),
            };
        }
    }
}
