using Assets;
using BomberEngine;
using BomberEngine.UI;
using Bomberman.Content;
using Bomberman.UI;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class SchemePickScreen : Screen
    {
        public enum ButtonId
        {
            Scheme,
            Back
        }

        private const int MapsPerPage = 6;

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

        private SchemeTableView m_table;

        public SchemePickScreen(ButtonDelegate buttonDelegate, int pageIndex, int selectedIndex)
        {
            m_table = new SchemeTableView(MapIDs, pageIndex, selectedIndex, buttonDelegate);
            m_table.screen = this;
            m_table.x = 64;
            m_table.y = 48;

            AddView(m_table);

            // controls
            View controls = new View(0.5f * width, 411, 0, 0);
            controls.alignX = View.ALIGN_CENTER;

            // hint left
            TextView hintView = new TextView(Helper.fontSystem, "Ctrl+Left");
            hintView.alignY = hintView.parentAlignY = View.ALIGN_CENTER;
            controls.AddView(hintView);

            // button
            Button button = new TempButton("BACK");
            button.id = (int)ButtonId.Back;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            controls.AddView(button);

            // hint right
            hintView = new TextView(Helper.fontSystem, "Ctrl+Right");
            hintView.alignY = hintView.parentAlignY = View.ALIGN_CENTER;
            controls.AddView(hintView);
            
            controls.LayoutHor(20);
            controls.ResizeToFitViews();
            AddView(controls);

            // focus
            FocusView(m_table);
        }

        public int pageIndex
        {
            get { return m_table.pageIndex; }
        }

        public int selectedIndex
        {
            get { return m_table.selectedIndex; }
        }
    }

    class SchemeTableView : View
    {
        private const int RowsPerPage = 2;
        private const int ColsPerPage = 3;
        private const int SchemesPerPage = RowsPerPage * ColsPerPage;

        private int[] m_ids;

        private int m_pageIndex;

        private RectView[] m_indicatorViews;
        private SchemeButton[] m_schemeButtons;

        private View m_contentView;
        private ButtonDelegate m_buttonDelegate;

        private int m_selectedIndex;
        public Screen screen;

        public SchemeTableView(int[] ids, int pageIndex, int selectedIndex, ButtonDelegate buttonDelegate)
        {
            m_ids = ids;
            m_schemeButtons = new SchemeButton[SchemesPerPage];
            m_buttonDelegate = buttonDelegate;

            // indicators
            View indicatorView = CreateIndicator(pagesCount);
            indicatorView.alignX = indicatorView.parentAlignX = View.ALIGN_CENTER;
            AddView(indicatorView);

            // maps
            m_contentView = new View(0, 0, 521, 323);
            m_contentView.debugColor = Color.Red;
            SetPage(pageIndex, selectedIndex);

            AddView(m_contentView);

            // arrange
            LayoutVer(10);
            ResizeToFitViews();
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = (KeyEvent)evt;
                if (keyEvent.IsKeyPressed(KeyCode.Left))
                {
                    if (keyEvent.IsCtrlPressed())
                    {
                        MovePage(-1);
                        return true;
                    }
                }

                if (keyEvent.IsKeyPressed(KeyCode.Right))
                {
                    if (keyEvent.IsCtrlPressed())
                    {
                        MovePage(1);
                        return true;
                    }
                }
            }

            return base.HandleEvent(evt);
        }

        private View CreateIndicator(int pagesCount)
        {
            View view = new View();
            m_indicatorViews = new RectView[pagesCount];
            for (int i = 0; i < m_indicatorViews.Length; ++i)
            {
                RectView r = new RectView(0, 0, 10, 10, Color.Transparent, Color.White);
                m_indicatorViews[i] = r;
                view.AddView(r);
            }
            view.LayoutHor(3);
            view.ResizeToFitViews();

            return view;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Paging

        private void SetPage(int pageIndex, int selectedIndex)
        {
            Debug.AssertRange(pageIndex, 0, pagesCount);
            Debug.AssertRange(selectedIndex, 0, SchemesPerPage);

            m_contentView.RemoveViews();
            m_pageIndex = pageIndex;
            m_selectedIndex = selectedIndex;

            int sw = 153;
            int sh = 143;

            float indent = (m_contentView.width - (ColsPerPage * sw)) / (ColsPerPage - 1);

            int index = 0;
            int arrayIndex = pageIndex * SchemesPerPage;

            ArrayUtils.Clear(m_schemeButtons);

            for (int i = 0; i < RowsPerPage && arrayIndex < m_ids.Length; ++i)
            {   
                for (int j = 0; j < ColsPerPage && arrayIndex < m_ids.Length; ++j)
                {
                    Scheme scheme = BmApplication.Assets().GetScheme(m_ids[arrayIndex]);

                    SchemeButton schemeView = new SchemeButton(scheme);
                    schemeView.x = j * (sw + indent);
                    schemeView.y = i * (sh + indent);
                    schemeView.id = arrayIndex;
                    schemeView.buttonDelegate = m_buttonDelegate;
                    m_schemeButtons[index] = schemeView;

                    m_contentView.AddView(schemeView);

                    ++arrayIndex;
                    ++index;
                }
            }
        }

        private bool MovePage(int delta)
        {   
            int oldPage = m_pageIndex;
            int newPage = MathHelp.ForceRange(m_pageIndex + delta, 0, pagesCount - 1);
            if (oldPage != newPage)
            {
                SetPage(newPage, 0);
                screen.FocusView(m_schemeButtons[0]);
                return true;
            }

            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Focus

        public override View FindFocusView(IFocusManager focusManager, FocusDirection direction)
        {
            focusManager.LockFocus(this);
            return m_schemeButtons[m_selectedIndex];
        }

        public override View FindFocusView(IFocusManager focusManager, View current, FocusDirection direction)
        {
            SchemeButton currentButton = current as SchemeButton;
            if (currentButton == null)
            {
                return FindFocusView(focusManager, direction);
            }

            int oldIndex = currentButton.id % SchemesPerPage;
            int dx = 0;
            int dy = 0;

            switch (direction)
            {
                case FocusDirection.Up:     dy = -1; break;
                case FocusDirection.Down:   dy = 1; break;
                case FocusDirection.Left:   dx = -1; break;
                case FocusDirection.Right:  dx = 1; break;
            }

            int col = ToCol(oldIndex);
            int row = ToRow(oldIndex);

            int newCol = MathHelp.ForceRange(col + dx, 0, ColsPerPage - 1);
            int newRow = MathHelp.ForceRange(row + dy, 0, RowsPerPage - 1);

            int newIndex = ToIndex(newRow, newCol);
            if (newIndex != oldIndex)
            {
                m_selectedIndex = newIndex;
                return m_schemeButtons[newIndex];
            }

            if (direction == FocusDirection.Down)
            {
                focusManager.UnlockFocus(this);

                View parent = Parent();
                return parent.FindFocusView(focusManager, this, direction);
            }
            
            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private int ToSchemeArrayIndex(int index)
        {
            return m_pageIndex * SchemesPerPage + index;
        }

        private int ToIndex(int row, int col)
        {
            return row * ColsPerPage + col;
        }

        private int ToRow(int index)
        {
            return index / ColsPerPage;
        }

        private int ToCol(int index)
        {
            return index % ColsPerPage;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public int pagesCount
        {
            get { return m_ids.Length / SchemesPerPage + (m_ids.Length % SchemesPerPage != 0 ? 1 : 0); }
        }

        public int pageIndex
        {
            get { return m_pageIndex; }
        }

        public int selectedIndex
        {
            get { return m_selectedIndex; }
        }

        #endregion
    }

    class SchemeButton : Button
    {
        private const int BorderSize = 3;

        private SchemeView m_schemeView;
        private Scheme m_scheme;

        public SchemeButton(Scheme scheme)
        {
            m_scheme = scheme;
            m_schemeView = new SchemeView(scheme, SchemeView.Style.Small);
            m_schemeView.x = m_schemeView.y = BorderSize;
            SetSize(m_schemeView.width + 2 * BorderSize, m_schemeView.height + 2 * BorderSize);
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            if (focused)
            {
                context.FillRect(0, 0, width - 1, height - 1, Color.Yellow);
            }
            m_schemeView.Draw(context);
            PostDraw(context);
        }

        public Scheme scheme
        {
            get { return m_scheme; }
        }
    }
}
