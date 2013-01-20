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

namespace Bomberman.Game.Scenes
{
    public class PowerupsDrawable : DrawableElement
    {   
        private TextureImage[] powerupImages;
        private Field field;
        
        public PowerupsDrawable(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;
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
                for (int i = 0; i < count; ++i)
                {   
                    if (drawX + image.GetWidth() > width)
                    {
                        drawX = 0;
                        drawY += image.GetHeight();
                    }

                    context.DrawImage(image, drawX, drawY);
                    drawX += image.GetWidth();
                }
                ++powerup;
            }

            PostDraw(context);
        }

        private void InitPowerupImages()
        {
            powerupImages = new TextureImage[]
            {
                Helper.GetTexture(A.tex_pow_bomb),
                Helper.GetTexture(A.tex_pow_flame),
                Helper.GetTexture(A.tex_pow_disea),
                Helper.GetTexture(A.tex_pow_kick),
                Helper.GetTexture(A.tex_pow_skate),
                Helper.GetTexture(A.tex_pow_punch),
                Helper.GetTexture(A.tex_pow_grab),
                Helper.GetTexture(A.tex_pow_pooge),
                Helper.GetTexture(A.tex_pow_gold),
                Helper.GetTexture(A.tex_pow_trig),
                Helper.GetTexture(A.tex_pow_jelly),
                Helper.GetTexture(A.tex_pow_ebola),
                Helper.GetTexture(A.tex_pow_rand),
            };
        }
    }
}
