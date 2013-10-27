using Assets;
using BomberEngine;
using Bomberman.Content;

namespace Bomberman.Game.Screens
{
    public class SchemeScreen : Screen
    {
        private static readonly int[] schemeIds = 
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

        public SchemeScreen()
        {
            Scheme scheme = BmApplication.Assets().GetScheme(A.maps_x);
            AddView(new SchemeView(scheme, SchemeView.Style.Small));
        }
    }
}
