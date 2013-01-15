using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using Assets;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game.Elements.Players;

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

            TempInitPlayerImages();
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
            
            TextureImage solidImage = Helper.GetTexture(A.tex_F0SOLID);
            TextureImage breakableImage = Helper.GetTexture(A.tex_F0BRICK);
            TextureImage bombImage = Helper.GetTexture(A.tex_BMB1001);

            foreach (FieldCell cell in cells)
            {
                TextureImage image = null;
                if (cell.IsBreakable())
                {
                    image = breakableImage;
                }
                else if (cell.IsSolid())
                {
                    image = solidImage;
                }
                else if (cell.IsBomb())
                {
                    image = bombImage;
                }

                if (image != null)
                {
                    float drawX = cell.GetPx() - 0.5f * image.GetWidth();
                    float drawY = cell.GetPy() - 0.5f * image.GetHeight();
                    context.DrawImage(image, drawX, drawY);
                }
            }
        }

        private void DrawPowerups(Context context)
        {

        }

        private void DrawPlayers(Context context)
        {
            List<Player> players = field.GetPlayers().GetList();
            foreach (Player player in players)
            {
                TextureImage image = TempFindPlayerImage(player.GetDirection());
                float drawX = player.GetPx() - 0.5f * cellWidth;
                float drawY = player.GetPy() - 0.5f * cellHeight;

                context.DrawRect(player.GetCx() * cellWidth, player.GetCy() * cellHeight, cellWidth, cellHeight, Color.White);
                context.DrawRect(drawX, drawY, cellWidth, cellHeight, Color.Yellow);

                context.DrawImage(image, drawX, drawY - image.GetHeight() + cellHeight);
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

        private TextureImage TempFindPlayerImage(Direction direction)
        {
            return playerImages[direction];
        }

        private void TempInitPlayerImages()
        {
            playerImages = new Dictionary<Direction, TextureImage>();
            playerImages.Add(Direction.DOWN, Helper.GetTexture(A.tex_WLKS0001));
            playerImages.Add(Direction.UP, Helper.GetTexture(A.tex_WLKN0001));
            playerImages.Add(Direction.LEFT, Helper.GetTexture(A.tex_WLKW0001));
            playerImages.Add(Direction.RIGHT, Helper.GetTexture(A.tex_WLKE0001));
        }
    }
}
