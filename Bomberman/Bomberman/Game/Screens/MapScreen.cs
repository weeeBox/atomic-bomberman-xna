using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Bomberman.UI;
using Assets;
using BombermanCommon.Resources;
using Bomberman.Content;

namespace Bomberman.Game.Screens
{
    public class MapScreen : Screen
    {
        public enum ButtonId
        {
            Continue,
            Back
        }

        private static readonly int[] MapIDs = 
        {
            A.maps_4corners,
		    A.maps_airchaos,
		    A.maps_airmail,
		    A.maps_alleys,
		    A.maps_alleys2,
		    A.maps_antfarm,
		    A.maps_asylum,
		    A.maps_back,
		    A.maps_back2,
		    A.maps_basic,
		    A.maps_basicsml,
		    A.maps_bman93,
		    A.maps_border,
		    A.maps_bowling,
		    A.maps_boxed,
		    A.maps_breakout,
		    A.maps_bunch,
		    A.maps_castle,
		    A.maps_castle2,
		    A.maps_chain,
		    A.maps_chase,
		    A.maps_checkers,
		    A.maps_chicane,
		    A.maps_clear,
		    A.maps_clearing,
		    A.maps_confused,
		    A.maps_cubic,
		    A.maps_cutter,
		    A.maps_cutthrot,
		    A.maps_deadend,
		    A.maps_diamond,
		    A.maps_dograce,
		    A.maps_dome,
		    A.maps_e_vs_w,
		    A.maps_fair,
		    A.maps_fargo,
		    A.maps_fort,
		    A.maps_freeway,
		    A.maps_gridlock,
		    A.maps_happy,
		    A.maps_jail,
		    A.maps_leak,
		    A.maps_neighbor,
		    A.maps_neil,
		    A.maps_n_vs_s,
		    A.maps_obstacle,
		    A.maps_og,
		    A.maps_pattern,
		    A.maps_pingpong,
		    A.maps_purist,
		    A.maps_racer1,
		    A.maps_rail1,
		    A.maps_railroad,
		    A.maps_roommate,
		    A.maps_r_garden,
		    A.maps_spiral,
		    A.maps_spread,
		    A.maps_tennis,
		    A.maps_thatthis,
		    A.maps_the_rim,
		    A.maps_thisthat,
		    A.maps_tight,
		    A.maps_toilet,
		    A.maps_uturn,
		    A.maps_volley,
		    A.maps_wallybom,
		    A.maps_x,
        };

        public MapScreen(ButtonDelegate buttonDelegate)
        {
            View contentView = new View(64, 48, 521, 363);

            int sw = 153;
            int sh = 143;
            float indent = (contentView.width - (3 * sw)) / 2;

            int index = 0;
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Scheme scheme = BmApplication.Assets().GetScheme(MapIDs[index]);
                    SchemeView schemeView = new SchemeView(scheme, SchemeView.Style.Small);
                    schemeView.x = j * (sw + indent);
                    schemeView.y = i * (sh + indent);
                    contentView.AddView(schemeView);

                    ++index;
                }
            }

            AddView(contentView);

            // buttons
            View buttons = new View(0.5f * width, contentView.y + contentView.height, 0, 0);
            buttons.alignX = View.ALIGN_CENTER;

            Button button = new TempButton("BACK");
            button.id = (int)ButtonId.Back;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            buttons.AddView(button);

            button = new TempButton("CONTINUE");
            button.id = (int)ButtonId.Continue;
            button.buttonDelegate = buttonDelegate;
            FocusView(button);
            SetConfirmButton(button);
            buttons.AddView(button);

            buttons.LayoutHor(20);
            buttons.ResizeToFitViews();
            AddView(buttons);
        }
    }
}
