using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using Assets;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldDrawable : DrawableElement
    {
        private Field field;

        private int cellWidth;
        private int cellHeight;

        public FieldDrawable(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;

            cellWidth = width / field.GetWidth();
            cellHeight = height / field.GetHeight();

            TempInitImages();
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            DrawGrid(context);
            DrawBlocks(context);
            DrawPowerups(context);
            DrawPlayers(context);

            PostDraw(context);
        }

        private void DrawBlocks(Context context)
        {
            FieldCell[] cells = field.GetCells().GetArray();

            foreach (FieldCell cell in cells)
            {
                TextureImage image = null;

                if (cell.IsBrick())
                {
                    image = breakableImage;
                }
                else if (cell.IsSolid())
                {
                    image = solidImage;
                }
                else if (cell.IsBomb())
                {
                    Bomb bomb = cell.AsBomb();
                    image = bombImage;
                    if (bomb.IsJelly())
                    {
                        image = bombJellyImage;
                    }
                    else if (bomb.IsTrigger())
                    {
                        image = bombTriggerImage;
                    }
                    context.DrawRect(cell.cx * cellWidth, cell.cy * cellHeight, cellWidth, cellHeight, Color.White);
                }
                else if (cell.IsPowerup())
                {
                    PowerupCell powerupCell = cell.AsPowerup();
                    int powerup = powerupCell.powerup;
                    if (powerup != Powerups.None)
                    {
                        if (powerup == Powerups.Random)
                        {
                            powerup = ((int)(powerupCell.elasped / 0.05f)) % powerupImages.Length;
                        }

                        image = powerupImages[powerup];
                    }
                }
                else if (cell.IsFlame())
                {
                    int x = cell.GetCx() * cellWidth;
                    int y = cell.GetCy() * cellHeight;
                    context.FillRect(x, y, cellWidth, cellHeight, Color.Red);
                }

                if (image != null)
                {
                    float drawX = cell.GetPx() - 0.5f * image.GetWidth();
                    float drawY = cell.GetPy() - 0.5f * image.GetHeight();
                    context.DrawImage(image, drawX, drawY);

                    if (cell.IsBrick())
                    {   
                        BrickCell brick = (BrickCell)cell;
                        
                        int powerup = brick.powerup;
                        if (powerup != Powerups.None)
                        {
                            TextureImage powerupImage = powerupImages[powerup];
                            drawX = cell.GetPx() - 0.5f * powerupImage.GetWidth();
                            drawY = cell.GetPy() - 0.5f * powerupImage.GetHeight();
                            context.DrawImage(powerupImage, drawX, drawY, 0.25f);
                        }                        
                    }
                }
            }
        }

        private void DrawPowerups(Context context)
        {

        }

        private void DrawPlayers(Context context)
        {
            List<Player> players = field.GetPlayers().list;
            foreach (Player player in players)
            {
                TextureImage image = TempFindPlayerImage(player);
                float drawX = player.GetPx() - 0.5f * cellWidth;
                float drawY = player.GetPy() - 0.5f * cellHeight;

                context.DrawRect(player.GetCx() * cellWidth, player.GetCy() * cellHeight, cellWidth, cellHeight, Color.White);
                context.DrawRect(drawX, drawY, cellWidth, cellHeight, Color.Yellow);

                context.DrawImage(image, drawX, drawY - image.GetHeight() + cellHeight);

                Bomb bomb = player.bombInHands;
                if (bomb != null)
                {
                    image = Helper.GetTexture(A.tex_BMB1001);

                    drawX = bomb.GetPx() - 0.5f * image.GetWidth();
                    drawY = bomb.GetPy() - 0.5f * image.GetHeight();
                    context.DrawImage(image, drawX, drawY);
                }

                List<Bomb> thrownBombs = player.thrownBombs;
                foreach (Bomb b in thrownBombs)
                {
                    image = Helper.GetTexture(A.tex_BMB1001);

                    drawX = b.GetPx() - 0.5f * image.GetWidth();
                    drawY = b.GetPy() - 0.5f * image.GetHeight() - b.fallHeight;
                    context.DrawImage(image, drawX, drawY);
                }
            }
        }

        private void DrawGrid(Context context)
        {
            for (int i = 0; i <= field.GetWidth(); ++i)
            {
                context.DrawLine(i * cellWidth, 0, i * cellWidth, height, Color.Gray);
            }

            for (int i = 0; i <= field.GetHeight(); ++i)
            {
                context.DrawLine(0, i * cellHeight, width, i * cellHeight, Color.Gray);
            }
        }

        private Dictionary<Direction, TextureImage> playerImages;
        private Dictionary<Direction, TextureImage> playerGrabImages;

        private TextureImage[] powerupImages;
        private TextureImage solidImage;
        private TextureImage breakableImage;
        private TextureImage bombImage;
        private TextureImage bombJellyImage;
        private TextureImage bombTriggerImage;

        private TextureImage TempFindPlayerImage(Player player)
        {
            Direction direction = player.GetDirection();
            if (player.IsHoldingBomb())
            {
                return playerGrabImages[direction];
            }

            return playerImages[direction];
        }

        private void TempInitImages()
        {
            playerImages = new Dictionary<Direction, TextureImage>();
            playerImages.Add(Direction.DOWN, Helper.GetTexture(A.tex_WLKS0001));
            playerImages.Add(Direction.UP, Helper.GetTexture(A.tex_WLKN0001));
            playerImages.Add(Direction.LEFT, Helper.GetTexture(A.tex_WLKW0001));
            playerImages.Add(Direction.RIGHT, Helper.GetTexture(A.tex_WLKE0001));

            playerGrabImages = new Dictionary<Direction, TextureImage>();
            playerGrabImages.Add(Direction.DOWN, Helper.GetTexture(A.tex_PUD));
            playerGrabImages.Add(Direction.UP, Helper.GetTexture(A.tex_PUP));
            playerGrabImages.Add(Direction.LEFT, Helper.GetTexture(A.tex_PUL));
            playerGrabImages.Add(Direction.RIGHT, Helper.GetTexture(A.tex_PUR));

            solidImage = Helper.GetTexture(A.tex_F0SOLID);
            breakableImage = Helper.GetTexture(A.tex_F0BRICK);
            bombImage = Helper.GetTexture(A.tex_BMB1001);
            bombJellyImage = Helper.GetTexture(A.tex_BMBC1);
            bombTriggerImage = Helper.GetTexture(A.tex_DIGIB001);

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
