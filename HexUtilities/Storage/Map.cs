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

namespace PGNapoleonics.HexUtilities.Storage {
    using MapGridHex = IHex;

    /// <summary>TODO</summary>
    public delegate MapDisplay<MapGridHex> MapExtractor();

    /// <summary>TODO</summary>
    public class Map {
        /// <summary>TODO</summary>
        public Map(string mapName, MapExtractor mapSource) {
            MapName   = mapName;
            MapSource = mapSource;
        }

        /// <summary>TODO</summary>
        public  string                 MapName   { get; }
        /// <summary>TODO</summary>
        public  MapDisplay<MapGridHex> MapBoard  => MapSource(); 
        
        private MapExtractor           MapSource { get; }

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is Map other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(Map other) => MapName == other.MapName;

        /// <inheritdoc/>
        public override int GetHashCode() => MapName.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(Map lhs, Map rhs) => ! lhs.Equals(rhs);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(Map lhs, Map rhs) => lhs.Equals(rhs);
        #endregion
    }
}
