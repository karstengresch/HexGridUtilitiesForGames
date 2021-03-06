﻿#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
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
using System.Collections;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;

#pragma warning disable 1587
/// <summary>Fast efficient <b>Shadow-Casting</b> 
/// implementation of 3D Field-of-View on a <see cref="Hexgrid"/> map.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.HexUtilities.FieldOfView {
    using HexSize = System.Drawing.Size;

    /// <summary>Implementation of <see cref="IShadingMask"/> using a backing array of BitArray.</summary>
    internal class ArrayFieldOfView : IShadingMask {
        private readonly object _syncLock = new object();

        public ArrayFieldOfView(IFovBoard board) {
            _mapSizeHexes = board.MapSizeHexes;
            _fovBacking   = ( from i in Enumerable.Range(0,board.MapSizeHexes.Width)
                              select new BitArray(board.MapSizeHexes.Height)
                            ).ToArray();
        }

        public bool this[HexCoords coords] {
            get => _mapSizeHexes.IsOnboard(coords) && _fovBacking[coords.User.X][coords.User.Y];
            internal set {
                lock (_syncLock) {
                    if (_mapSizeHexes.IsOnboard(coords)) { _fovBacking[coords.User.X][coords.User.Y] = value; }
                }
            }
        }
        readonly BitArray[] _fovBacking;

        private readonly HexSize _mapSizeHexes;
    }
}
