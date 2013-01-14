using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;

namespace Bomberman.Game.Scenes
{
    public class GameScene : Scene
    {
        private Image fieldBackground;

        private Field field;

        public GameScene()
        {
            field = LoadField();

            KeyboardPlayerInput input = new KeyboardPlayerInput();
            Application.Input().SetKeyboardListener(input);

            Player player = new Player(input, 0, 0);
            field.AddPlayer(player);

            AddUpdatabled(field);

            // field background
            fieldBackground = Helper.CreateImage(A.tex_FIELD7);
            AddDrawable(fieldBackground);

            // field drawer
            AddDrawable(new FieldDrawable(field, Constant.FIELD_OFFSET_X, Constant.FIELD_OFFSET_Y, Constant.FIELD_WIDTH, Constant.FIELD_HEIGHT));
        }

        private Field LoadField()
        {
            int fieldWidth = FIELD_DATA[0].Length;
            int fieldHeight = FIELD_DATA.Length;

            Field field = new Field(fieldWidth, fieldHeight);
            FieldCellArray cells = field.GetCells();

            for (int y = 0; y < FIELD_DATA.Length; ++y)
            {
                String data = FIELD_DATA[y];
                for (int x = 0; x < data.Length; ++x)
                {
                    char chr = data[x];
                    switch (chr)
                    {
                        case '.':
                        {
                            cells.Set(x, y, new EmptyCell(x, y));
                            break;
                        }

                        case '#':
                        {
                            cells.Set(x, y, new BrickCell(x, y, true));
                            break;
                        }

                        case ':':
                        {
                            cells.Set(x, y, new BrickCell(x, y, false));
                            break;
                        }
                    }
                }
            }

            return field;
        }

        private static String[] FIELD_DATA =
        {
            ".....#:::#.....",
            "#.....#:#.....#",
            ":#.....:.....#:",
            "::#.........#::",
            ":::#.......#:::",
            "::::.......::::",
            ":::#.......#:::",
            "::#.........#::",
            ":#.....:.....#:",
            "#.....#:#.....#",
            ".....#:::#....."
        };
    }
}
