﻿using System;
using System.Collections.Generic;
using Assets;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay.Elements.Cells;
using Bomberman.Gameplay.Elements.Players;
using Microsoft.Xna.Framework;

namespace Bomberman.Gameplay.Elements.Fields
{
    public class FieldDrawable : View, IFieldDrawable
    {
        private Field field;

        private int cellWidth;
        private int cellHeight;

        private bool blink;
        private IDictionary<Direction, TextureImage> dirLookup;

        private const int anim_flame_center_green = 0;
		private const int anim_flame_mideast_green = 1;
		private const int anim_flame_midnorth_green = 2;
		private const int anim_flame_midsouth_green = 3;
		private const int anim_flame_midwest_green = 4;
		private const int anim_flame_tipeast_green = 5;
		private const int anim_flame_tipnorth_green = 6;
		private const int anim_flame_tipsouth_green = 7;
        private const int anim_flame_tipwest_green = 8;

        private AnimationInstance[] flameAnimations = 
        {
            new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_center_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_mideast_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_midnorth_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_midsouth_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_midwest_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_tipeast_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_tipnorth_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_tipsouth_green)),
		    new AnimationInstance(BmApplication.Assets().GetAnimation(A.anim_flame_tipwest_green)),
        };

        private int[] flameAnimationsMidIdx =
        {
            anim_flame_midnorth_green,
            anim_flame_midsouth_green,
            anim_flame_midwest_green,
            anim_flame_mideast_green,
        };

        private int[] flameAnimationsTipIdx =
        {
            anim_flame_tipnorth_green,
            anim_flame_tipsouth_green,
            anim_flame_tipwest_green,
            anim_flame_tipeast_green,
        };

        private List<PlayerDrawable> m_playersDrawables;

        public FieldDrawable(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;

            cellWidth = width / field.GetWidth();
            cellHeight = height / field.GetHeight();

            TempInitImages();

            field.ScheduleTimer(ToggleBlink, 0.01f, true);
            field.ScheduleTimer(UpdateFlameAnimation, 0.0f, true);

            m_playersDrawables = new List<PlayerDrawable>(10);
        }

        private void ToggleBlink()
        {
            blink = !blink;
        }

        private void UpdateFlameAnimation()
        {
            float delta = Application.frameTime;
            for (int i = 0; i < flameAnimations.Length; ++i)
            {
                flameAnimations[i].Update(delta);
            }
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            DrawGrid(context);
            DrawCells(context);
            DrawSpecial(context);

            PostDraw(context);
        }

        private void DrawSpecial(Context context)
        {
            List<Player> players = field.GetPlayers().list;
            foreach (Player player in players)
            {
                DrawSpecial(context, player);
            }
        }

        private void DrawSpecial(Context context, Player player)
        {
            Bomb bomb = player.bombInHands;
            if (bomb != null)
            {
                TextureImage image = Helper.GetTexture(A.gfx_bmb1001);

                float drawX = player.GetPx() - 0.5f * image.GetWidth();
                float drawY = player.GetPy() - 1.5f * image.GetHeight();
                context.DrawImage(image, drawX, drawY);
            }

            List<Bomb> thrownBombs = player.thrownBombs;
            foreach (Bomb b in thrownBombs)
            {
                TextureImage image = Helper.GetTexture(A.gfx_bmb1001);

                float drawX = b.GetPx() - 0.5f * image.GetWidth();
                float drawY = b.GetPy() - 0.5f * image.GetHeight() - b.fallHeight;
                context.DrawImage(image, drawX, drawY);
            }
        }

        private void DrawCells(Context context)
        {
            FieldCellSlot[] slots = field.GetSlots();

            bool drawSlotSize = CVars.g_drawSlotSize.boolValue;
            foreach (FieldCellSlot slot in slots)
            {
                FieldCell cell = slot.staticCell;
                if (cell != null)
                {
                    DrawCell(context, cell);
                }

                if (slot.MovableCount() > 0)
                {
                    foreach (MovableCell movableCell in slot.movableCells)
                    {
                        DrawCell(context, movableCell);
                    }
                }

                if (slot.Size() > 0 && drawSlotSize)
                {
                    float drawX = Util.Cx2Px(slot.cx) - 0.5f * cellWidth;
                    float drawY = Util.Cy2Py(slot.cy) - 0.5f * cellHeight;
                    context.DrawString(drawX, drawY, "" + slot.Size());
                }
            }
        }
        
        private void DrawCell(Context context, FieldCell cell)
        {
            if (cell.IsBrick())
            {
                DrawBrick(context, cell.AsBrick());
            }
            else if (cell.IsSolid())
            {
                DrawSolid(context, cell);
            }
            else if (cell.IsBomb())
            {
                DrawBomb(context, cell.AsBomb());
            }
            else if (cell.IsFlame())
            {
                DrawFlame(context, cell.AsFlame());
            }
            else if (cell.IsPowerup())
            {
                DrawPowerup(context, cell.AsPowerup());
            }
            else if (cell.IsPlayer())
            {
                DrawPlayer(context, cell.AsPlayer());
            }
        }   

        private void DrawBrick(Context context, BrickCell cell)
        {
            DrawCellImage(context, cell, breakableImage, cell.hit ? Color.Red : Color.White);

            if (CVars.g_drawHiddenPowerups.boolValue)
            {
                int powerup = cell.powerup;
                if (powerup != Powerups.None)
                {
                    TextureImage powerupImage = powerupImages[powerup];
                    float drawX = cell.GetPx() - 0.5f * powerupImage.GetWidth();
                    float drawY = cell.GetPy() - 0.5f * powerupImage.GetHeight();
                    context.DrawImage(powerupImage, drawX, drawY, 0.25f);
                }
            }
        }

        private void DrawSolid(Context context, FieldCell cell)
        {
            DrawCellImage(context, cell, solidImage);
        }

        private void DrawBomb(Context context, Bomb bomb)
        {
            float drawX = bomb.GetPx();
            float drawY = bomb.GetPy();

            BombDrawable drawable = (BombDrawable)bomb.BombDrawable;
            drawable.Draw(context, drawX, drawY + 0.5f * cellHeight);

            if (CVars.g_drawBombDir.boolValue)
            {
                TextureImage dirImage = dirLookup[bomb.direction];
                context.DrawImage(dirImage, drawX - 0.5f * dirImage.GetWidth(), drawY - 0.5f * dirImage.GetHeight());
            }
            
            context.DrawRect(bomb.cx * cellWidth, bomb.cy * cellHeight, cellWidth, cellHeight, Color.White);
            context.DrawRect(bomb.px - 0.5f * cellWidth, bomb.py - 0.5f * cellHeight, cellWidth, cellHeight, Color.Red);
        }

        private void DrawFlame(Context context, FlameCell flame)
        {
            int index = 0;
            if (flame.isCenter)
            {
                index = anim_flame_center_green;
            }
            else
            {
                int[] idxs = flame.isCap ? flameAnimationsTipIdx : flameAnimationsMidIdx;
                index = idxs[(int)flame.direction];
            }

            AnimationInstance anim = flameAnimations[index];
            anim.Draw(context, flame.px, flame.py);
        }

        private void DrawPowerup(Context context, PowerupCell powerupCell)
        {   
            int powerup = powerupCell.powerup;
            if (powerup != Powerups.None)
            {
                if (powerup == Powerups.Random)
                {
                    powerup = ((int)(powerupCell.elasped / 0.05f)) % powerupImages.Length;
                }

                TextureImage image = powerupImages[powerup];
                DrawCellImage(context, powerupCell, image);
            }
        }

        private void DrawPlayer(Context context, Player player)
        {
            TextureImage image = TempFindPlayerImage(player);
            float drawX = player.GetPx() - 0.5f * cellWidth;
            float drawY = player.GetPy() - 0.5f * cellHeight;

            if (CVars.g_drawPlayerCell.boolValue)
            {
                context.DrawRect(player.GetCx() * cellWidth, player.GetCy() * cellHeight, cellWidth, cellHeight, Color.White);
            }

            if (CVars.g_drawPlayerMovable.boolValue)
            {
                context.DrawRect(drawX, drawY, cellWidth, cellHeight, Color.Yellow);
            }

            PlayerDrawable drawable = (PlayerDrawable)player.PlayerDrawable;
            if (player.IsInfected())
            {
                if (blink)
                {   
                    drawable.Draw(context, drawX + 0.5f * cellWidth, drawY + cellHeight);
                }
            }
            else
            {   
                drawable.Draw(context, drawX + 0.5f * cellWidth, drawY + cellHeight);
            }

            if (CVars.g_drawPlayerStepRect.boolValue)
            {
                int stepX = Math.Sign(player.px - player.CellCenterPx());
                int stepY = Math.Sign(player.py - player.CellCenterPy());

                bool hasStepX = stepX != 0;
                bool hasStepY = stepY != 0;

                int cx = player.GetCx();
                int cy = player.GetCy();

                if (hasStepX && hasStepY)
                {
                    DrawCellRect(context, cx + stepX, cy, Color.Yellow);
                    DrawCellRect(context, cx, cy + stepY, Color.Yellow);
                    DrawCellRect(context, cx + stepX, cy + stepY, Color.Yellow);
                }
                else if (hasStepX)
                {
                    DrawCellRect(context, cx + stepX, cy, Color.Yellow);
                }
                else if (hasStepY)
                {
                    DrawCellRect(context, cx, cy + stepY, Color.Yellow);
                }
            }
        }

        private void DrawCellImage(Context context, FieldCell cell, TextureImage image)
        {
            DrawCellImage(context, cell, image, color);
        }

        private void DrawCellImage(Context context, FieldCell cell, TextureImage image, Color color)
        {
            float drawX = cell.GetPx() - 0.5f * image.GetWidth();
            float drawY = cell.GetPy() - 0.5f * image.GetHeight();
            context.DrawImage(image, drawX, drawY, color);
        }

        private void DrawCellRect(Context context, int cx, int cy, Color color)
        {
            context.DrawRect(cx * cellWidth, cy * cellHeight, cellWidth, cellHeight, color);
        }

        private void DrawCellRect(Context context, FieldCell cell, Color color)
        {
            DrawCellRect(context, cell.GetCx(), cell.GetCy(), color);
        }
        
        private void DrawGrid(Context context)
        {
            if (CVars.g_drawGrid.boolValue)
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
        }

        //////////////////////////////////////////////////////////////////////////////

        public void AddPlayer(Player player)
        {
            Assert.IsTrue(IndexOf(player) == -1);
            PlayerDrawable drawable = new PlayerDrawable(player);
            player.PlayerDrawable = drawable;

            m_playersDrawables.Add(drawable);
        }

        public void RemovePlayer(Player player)
        {
            int index = IndexOf(player);
            m_playersDrawables.RemoveAt(index);
        }

        public void Reset()
        {
            m_playersDrawables.Clear();
        }

        private int IndexOf(Player player)
        {
            for (int i = 0; i < m_playersDrawables.Count; ++i)
            {
                if (m_playersDrawables[i].Player == player)
                {
                    return i;
                }
            }

            return -1;
        }

        //////////////////////////////////////////////////////////////////////////////

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
            Direction direction = player.direction;
            if (player.IsHoldingBomb())
            {
                return playerGrabImages[direction];
            }

            return playerImages[direction];
        }

        private void TempInitImages()
        {
            playerImages = new Dictionary<Direction, TextureImage>();
            playerImages.Add(Direction.DOWN, Helper.GetTexture(A.gfx_wlks0001));
            playerImages.Add(Direction.UP, Helper.GetTexture(A.gfx_wlkn0001));
            playerImages.Add(Direction.LEFT, Helper.GetTexture(A.gfx_wlkw0001));
            playerImages.Add(Direction.RIGHT, Helper.GetTexture(A.gfx_wlke0001));

            playerGrabImages = new Dictionary<Direction, TextureImage>();
            playerGrabImages.Add(Direction.DOWN, Helper.GetTexture(A.gfx_bmf0010));
            playerGrabImages.Add(Direction.UP, Helper.GetTexture(A.gfx_bmf0010));
            playerGrabImages.Add(Direction.LEFT, Helper.GetTexture(A.gfx_pul0010));
            playerGrabImages.Add(Direction.RIGHT, Helper.GetTexture(A.gfx_pur0010));

            solidImage = Helper.GetTexture(A.gfx_f0solid);
            breakableImage = Helper.GetTexture(A.gfx_f0brick);
            bombImage = Helper.GetTexture(A.gfx_bmb1001);
            bombJellyImage = Helper.GetTexture(A.gfx_bmbc1);
            bombTriggerImage = Helper.GetTexture(A.gfx_digib001);

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

            dirLookup = new Dictionary<Direction, TextureImage>();
            dirLookup[Direction.UP] = Helper.GetTexture(A.gfx_dir_up);
            dirLookup[Direction.DOWN] = Helper.GetTexture(A.gfx_dir_down);
            dirLookup[Direction.LEFT] = Helper.GetTexture(A.gfx_dir_left);
            dirLookup[Direction.RIGHT] = Helper.GetTexture(A.gfx_dir_right);
        }
    }
}
