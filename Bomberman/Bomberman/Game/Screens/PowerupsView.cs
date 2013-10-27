using Assets;
using BomberEngine;
using BomberEngine.UI;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Players;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class PowerupsView : View
    {
        private PowerupView[] m_powerupViews;

        private PowerupList m_powerups;
        private Color m_innactiveColor;

        public PowerupsView(PowerupList powerups)
        {
            m_powerups = powerups;
            m_innactiveColor = new Color(0, 0, 0, 0.5f);

            TextureImage[] images = InitPowerupImages();
            
            m_powerupViews = new PowerupView[images.Length];
            for (int i = 0; i < images.Length; ++i)
            {
                PowerupView pw = new PowerupView(images[i], powerups.GetCount(i));
                AddView(pw, i * pw.width, 0.0f);
                m_powerupViews[i] = pw;
            }

            ResizeToFitViews();
        }

        public override void Update(float delta)
        {   
            for (int i = 0; i < Powerups.Count; ++i)
            {
                m_powerupViews[i].Count = m_powerups.GetCount(i);
            }
        }

        private TextureImage[] InitPowerupImages()
        {
            return new TextureImage[]
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

    internal class PowerupView : View
    {
        private int m_count;
        private TextView m_countTextView;
        private RectView m_dimmingView;

        public PowerupView(TextureImage tex, int count)
            : base(tex.GetWidth(), tex.GetHeight())
        {
            ImageView view = new ImageView(tex);
            AddView(view);

            m_countTextView = new TextView(Helper.fontSystem, "");
            m_countTextView.backColor = Color.Black;
            AddView(m_countTextView);

            m_dimmingView = new RectView(0, 0, width, height, new Color(0, 0, 0, 0.5f), Color.Black);
            AddView(m_dimmingView);

            SetCount(count);
        }

        public int Count
        {
            get { return m_count; }
            set
            {
                if (m_count != value)
                {
                    SetCount(value);
                }
            }
        }

        private void SetCount(int count)
        {
            m_countTextView.text = count.ToString();
            m_count = count;

            m_countTextView.visible = m_count > 0;
            m_dimmingView.visible = m_count == 0;
        }
    }
}
