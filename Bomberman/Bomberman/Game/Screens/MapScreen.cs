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
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace Bomberman.Game.Screens
{
    public class MapScreen : Screen
    {
        public enum ButtonId
        {
            Continue,
            Back
        }

        private static readonly String KeyLastPageIndex = "LastPageIndex";
        private static readonly String KeyLastMapIndex = "LastMapIndex";

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

        public MapScreen(ButtonDelegate buttonDelegate)
        {
            int pageIndex = Application.Storage().GetInt(KeyLastPageIndex);
            int mapIndex = Application.Storage().GetInt(KeyLastMapIndex);

            SchemeTableView table = new SchemeTableView(MapIDs, pageIndex, mapIndex);
            table.x = 64;
            table.y = 48;

            AddView(table);

            // buttons
            View buttons = new View(0.5f * width, 411, 0, 0);
            buttons.alignX = View.ALIGN_CENTER;

            Button button = new TempButton("BACK");
            button.id = (int)ButtonId.Back;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            buttons.AddView(button);

            button = new TempButton("CONTINUE");
            button.id = (int)ButtonId.Continue;
            button.buttonDelegate = buttonDelegate;
            SetConfirmButton(button);
            buttons.AddView(button);

            buttons.LayoutHor(20);
            buttons.ResizeToFitViews();
            AddView(buttons);

            FocusView(table);
        }
    }

    class SchemeTableView : View
    {
        private const int RowsPerPage = 2;
        private const int ColsPerPage = 3;
        private const int SchemesPerPage = RowsPerPage * ColsPerPage;

        private int[] m_ids;

        private int m_firstVisibleIndex;
        private int m_selectedIndex;

        private RectView[] m_indicatorViews;
        private SchemeView[] m_schemeViews;

        private View m_contentView;

        private int m_rowsCount;
        private int m_colsCount;

        public SchemeTableView(int[] ids, int firstVisibleIndex, int selectedIndex)
        {
            focusable = true;

            m_ids = ids;
            m_rowsCount = RowsPerPage;
            m_colsCount = m_ids.Length / m_rowsCount + (m_ids.Length % m_rowsCount != 0 ? 1 : 0);
            m_schemeViews = new SchemeView[SchemesPerPage];

            int pagesCount = (ids.Length / SchemesPerPage) + (ids.Length % SchemesPerPage != 0 ? 1 : 0);

            // indicators
            View indicatorView = CreateIndicator(pagesCount);
            indicatorView.alignX = indicatorView.parentAlignX = View.ALIGN_CENTER;
            AddView(indicatorView);

            // maps
            m_contentView = new View(0, 0, 521, 0);
            m_contentView.debugColor = Color.Red;
            UpdateMaps(firstVisibleIndex);
            m_contentView.ResizeToFitViews();

            m_schemeViews[0].focused = true;

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
                if (keyEvent.IsKeyPressed(KeyCode.Up))
                {
                    Move(0, -1);
                    return true;
                }

                if (keyEvent.IsKeyPressed(KeyCode.Down))
                {
                    Move(0, 1);
                    return true;
                }

                if (keyEvent.IsKeyPressed(KeyCode.Left))
                {
                    Move(-1, 0);
                    return true;
                }

                if (keyEvent.IsKeyPressed(KeyCode.Right))
                {
                    Move(1, 0);
                    return true;
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

        private void UpdateMaps(int startIndex)
        {
            m_contentView.RemoveViews();

            int sw = 153;
            int sh = 143;

            float indent = (m_contentView.width - (ColsPerPage * sw)) / (ColsPerPage - 1);

            int startRow = ToRow(startIndex);
            int startCol = ToCol(startIndex);

            int index = 0;

            ArrayUtils.Clear(m_schemeViews);

            for (int i = 0; i < RowsPerPage; ++i)
            {
                int schemeIndex = ToIndex(startRow + i, startCol);
                for (int j = 0; j < ColsPerPage; ++j)
                {
                    if (schemeIndex == m_ids.Length)
                    {
                        return;
                    }

                    Scheme scheme = BmApplication.Assets().GetScheme(m_ids[schemeIndex]);

                    SchemeView schemeView = new SchemeView(scheme, SchemeView.Style.Small);
                    schemeView.x = j * (sw + indent);
                    schemeView.y = i * (sh + indent);
                    schemeView.id = schemeIndex;
                    m_schemeViews[index] = schemeView;

                    m_contentView.AddView(schemeView);

                    ++schemeIndex;
                    ++index;
                }
            }
        }

        private bool Move(int dx, int dy)
        {
            int col = ToCol(m_selectedIndex);
            int row = ToRow(m_selectedIndex);

            int newCol = MathHelp.ForceRange(col + dx, 0, m_colsCount - 1);
            int newRow = MathHelp.ForceRange(row + dy, 0, m_rowsCount - 1);

            int newIndex = ToIndex(newRow, newCol);
            if (newIndex != m_selectedIndex)
            {
                int oldSchemeIndex = ToSchemeArrayIndex(m_selectedIndex);
                Debug.Assert(m_schemeViews[oldSchemeIndex].focused);
                m_schemeViews[oldSchemeIndex].focused = false;

                if (dx != 0)
                {
                    int firstCol = ToCol(m_firstVisibleIndex);
                    int diff = newCol - firstCol;
                    if (diff >= ColsPerPage)
                    {
                        m_firstVisibleIndex = ToIndex(0, newCol-(ColsPerPage-1));
                        UpdateMaps(m_firstVisibleIndex);
                    }
                    else if (diff <= -1)
                    {
                        m_firstVisibleIndex = ToIndex(0, newCol);
                        UpdateMaps(m_firstVisibleIndex);
                    }
                }

                int newSchemeIndex = ToSchemeArrayIndex(newIndex);
                Debug.Assert(!m_schemeViews[newSchemeIndex].focused);
                m_schemeViews[newSchemeIndex].focused = true;

                m_selectedIndex = newIndex;
                return true;
            }

            return false;
        }

        private int ToSchemeArrayIndex(int index)
        {
            int row = ToRow(index);
            int col = ToCol(index);
            int firstCol = ToCol(m_firstVisibleIndex);

            return row * ColsPerPage + (col - firstCol);
        }

        private int ToIndex(int row, int col)
        {
            return row * m_colsCount + col;
        }

        private int ToRow(int index)
        {
            return index / m_colsCount;
        }

        private int ToCol(int index)
        {
            return index % m_colsCount;
        }

        //private void Next()
        //{
        //    int newIndex = m_pageIndex + MapsPerPage;
        //    if (newIndex < MapIDs.Length)
        //    {
        //        SetPage(newIndex);
        //    }
        //}

        //private void Prev()
        //{
        //    int newIndex = m_pageIndex - MapsPerPage;
        //    if (newIndex >= 0)
        //    {
        //        SetPage(newIndex);
        //    }
        //}

        //private void SetPage(int index)
        //{
        //    int oldIndex = index;
        //    m_pageIndex = index;
        //    FillMaps(m_pageIndex);

        //    m_pageViews[oldIndex].fillColor = Color.Transparent;
        //    m_pageViews[index].fillColor = Color.Yellow;

        //    Application.Storage().Set(KeyLastPageIndex, m_pageIndex);
        //}
    }
}
