﻿#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FieldOfView;
using PGNapoleonics.HexgridPanel;
using PGNapoleonics.WinForms;

using PGNapoleonics.HexgridExampleCommon;

namespace PGNapoleonics.HexgridPanelExample {
    using HexSize        = System.Drawing.Size;
    using MapGridDisplay = MapDisplay<IHex>;

    internal sealed partial class HexgridPanelExample : Form, IMessageFilter {
        private bool           _isPanelResizeSuppressed = false;
        private MapGridDisplay _mapBoard;
        private CustomCoords   _customCoords;

        public HexgridPanelExample() {
            InitializeComponent();
            Application.AddMessageFilter(this);

            //var resources = new ComponentResourceManager(typeof(HexgridPanelExample));

            _hexgridPanel.ScaleChange += new EventHandler<EventArgs>((o,e) => OnResizeEnd(e));

             LoadTraceMenu(menuItemDebugTracing_Click);

            comboBoxMapSelection.Items.AddRange(Map.MapList.Select(item => item.MapName).ToArray());
            comboBoxMapSelection.SelectedIndex = 0;

    //      helpProvider1.SetShowHelp(this,true);
        }
        protected override CreateParams CreateParams => this.SetCompositedStyle(base.CreateParams);

        [Conditional("TRACE")]
        void LoadTraceMenu(EventHandler handler) =>
            Tracing.ForEachKey(item => LoadTraceMenuItem(item,handler), n => n!="None");

        [Conditional("TRACE")]
        void LoadTraceMenuItem(string item, EventHandler handler) {
            var menuItem = new ToolStripMenuItem();
            menuItemDebug.DropDownItems.Add(menuItem);
            menuItem.Name         = "menuItemDebugTracing" + item.ToString();
            menuItem.Size         = new HexSize(143, 22);
            menuItem.Text         = item.ToString();
            menuItem.CheckOnClick = true;
            menuItem.Click       += handler;
        }

        private void LoadLandmarkMenu() {
            menuItemLandmarks.Items.Clear();
            menuItemLandmarks.Items.Add("None");
            if (_mapBoard.Landmarks != null) {
                _mapBoard.Landmarks.ForEach(landmark => menuItemLandmarks.Items.Add($"{landmark.Coords}"));
            }

            menuItemLandmarks.SelectedIndexChanged += new EventHandler(menuItemLandmarks_SelectedIndexChanged);
            menuItemLandmarks.SelectedIndex = 0; 
        }

        #region Event handlers
        private void HexGridExampleForm_Load(object sender, EventArgs e) {
            _hexgridPanel.ScaleIndex = _hexgridPanel.ScaleList
                                                    .Select((f,i) => new {value=f, index=i})
                                                    .Where(s => s.value==1.0F)
                                                    .Select(s => s.index).FirstOrDefault(); 
            var padding = toolStripContainer1.ContentPanel.Padding;
            Size = _hexgridPanel.MapSizePixels  + new HexSize(21,93)
                 + new HexSize(padding.Left+padding.Right, padding.Top+padding.Bottom);
        }

        protected override void OnResizeBegin(EventArgs e) {
            base.OnResizeBegin(e);
            _isPanelResizeSuppressed = true;
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (IsHandleCreated && ! _isPanelResizeSuppressed) _hexgridPanel.SetScrollLimits(_mapBoard);
        }
        protected override void OnResizeEnd(EventArgs e) {
            base.OnResizeEnd(e);
            _isPanelResizeSuppressed = false;
            _hexgridPanel.SetScrollLimits(_mapBoard);
        }

        private void hexgridPanel_MouseMove(object sender, MouseEventArgs e) {
            var hotHex       = _mapBoard.HotspotHex;
            statusLabel.Text = string.Format(_customCoords,
                Properties.Resources.StatusLabelText,
                hotHex, hotHex, hotHex,
                _mapBoard.StartHex - hotHex, _mapBoard.Path.Match(path=>path.TotalCost, ()=>0))
            +
                $"  Elevation: Ground={_mapBoard.ElevationGroundASL(hotHex)};" +
                $" Observer={_mapBoard.ElevationObserverASL(hotHex)};" +
                $" Target={_mapBoard.ElevationTargetASL(hotHex)}"; 
        }

        private void txtPathCutover_TextChanged(object sender, EventArgs e) {
            if(int.TryParse(txtPathCutover.Text, out var value)) {
                txtPathCutover.Tag = value;
            } else {
                txtPathCutover.Text = txtPathCutover.Tag.ToString();
                value = (int)txtPathCutover.Tag;
            }
            _mapBoard.FovRadius   =
            _mapBoard.RangeCutoff = value;
            Refresh();
        }

        private void menuItemLandmarks_SelectedIndexChanged(object sender, EventArgs e) {
            _mapBoard.LandmarkToShow = menuItemLandmarks.SelectedIndex;
            _hexgridPanel.SetMapDirty();
            Update();
        }

        private void menuItemDebugTracing_Click(object sender, EventArgs e) {
            var item = (ToolStripMenuItem)sender;
            item.CheckState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
            var name = item.Name.Replace("menuItemDebugTracing","");
            var flag = Tracing.Item(name);
            if( item.Checked)   _enabledTraces |=  flag;
            else                _enabledTraces &= ~flag;
        }

        private Tracing _enabledTraces = Tracing.None;

        private void menuItemHelpContents_Click(object sender, EventArgs e) {
    //      helpProvider1.SetShowHelp(this,true);
        }

        private void comboBoxMapSelection_SelectionChanged(object sender, EventArgs e)
        =>  SetMapBoard(ParseMapName(((ToolStripItem)sender).Text));
        
        private static MapGridDisplay ParseMapName(string mapName)
        =>  Map.MapList.First(item => item.MapName == mapName).MapBoard;

        private void SetMapBoard(MapGridDisplay mapBoard) {
            _hexgridPanel.Model     = _mapBoard
                                    = mapBoard;
            _mapBoard.ShowPathArrow = buttonPathArrow.Checked;
            _mapBoard.ShowFov       = buttonFieldOfView.Checked;
            _mapBoard.FovRadius     =
            _mapBoard.RangeCutoff   = Int32.Parse(txtPathCutover.Tag.ToString(),CultureInfo.InvariantCulture);
            LoadLandmarkMenu();

            _customCoords = new CustomCoords(new IntMatrix2D(2,0, 0,-2, 0,2*_mapBoard.MapSizeHexes.Height-1, 2));
            _hexgridPanel.Focus();
       }

        private void buttonFieldOfView_Click(object sender, EventArgs e) {
            _mapBoard.ShowFov = buttonFieldOfView.Checked;
            _hexgridPanel.Refresh();
        }
        private void buttonPathArrow_Click(object sender, EventArgs e) {
            _mapBoard.ShowPathArrow = buttonPathArrow.Checked;
            _hexgridPanel.Refresh();
        }
        private void buttonRangeLine_Click(object sender, EventArgs e) {
            _mapBoard.ShowRangeLine = buttonRangeLine.Checked;
            _hexgridPanel.SetMapDirty();
            _mapBoard.StartHex = _mapBoard.StartHex; // Indirect, but it works.
            _hexgridPanel.Refresh();
        }
        private void buttonTransposeMap_Click(object sender, EventArgs e) {
            _hexgridPanel.IsTransposed = buttonTransposeMap.Checked;
        }

        private void PanelBoard_GoalHexChange(object sender, HexEventArgs e) {
            _mapBoard.GoalHex = e.Coords;
            _hexgridPanel.Refresh();
        }
        private void PanelBoard_StartHexChange(object sender, HexEventArgs e) {
            _mapBoard.StartHex = e.Coords;
            _hexgridPanel.Refresh();
        }
        private void PanelBoard_HotSpotHexChange(object sender, HexEventArgs e) {
            _mapBoard.HotspotHex = e.Coords;
            _hexgridPanel.Refresh();
        }
        #endregion

        #region IMessageFilter implementation
        /// <summary>Redirect WM_MouseWheel messages to window under mouse.</summary>
        /// <remarks>Redirect WM_MouseWheel messages to window under mouse (rather than 
        /// that with focus) with adjusted delta.
        /// <a href="http://www.flounder.com/virtual_screen_coordinates.htm">Virtual Screen Coordinates</a>
        /// Dont forget to add this to constructor:
        /// 			Application.AddMessageFilter(this);
        ///</remarks>
        /// <param name="m">The Windows Message to filter and/or process.</param>
        /// <returns>Success (true) or failure (false) to OS.</returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool PreFilterMessage(ref Message m) {
            if ((WM)m.Msg != WM.MouseHWheel && (WM)m.Msg != WM.MouseWheel) return false;

            var hWnd = NativeMethods.WindowFromPoint(WindowsMouseInput.GetPointLParam(m.LParam));
            var ctl = FromChildHandle(hWnd);
            if (hWnd != IntPtr.Zero  &&  hWnd != m.HWnd  &&  ctl != null) {
                switch ((WM)m.Msg) {
                    case WM.MouseHWheel:
                    case WM.MouseWheel:
                        Tracing.ScrollEvents.Trace(true, $" - {Name}.WM.{(WM)m.Msg}: ");
                        return (NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam) == IntPtr.Zero);
                    default: break;
                }
            }
            return false;
        }
        #endregion
    }
}