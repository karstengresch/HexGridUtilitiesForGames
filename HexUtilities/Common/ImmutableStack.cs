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
using System.Collections.Generic;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>Eric Lippert's implementation for use in A*.</summary>
    /// <remarks>An implementation of immutable stack for use in A* as a 'Path to here'..</remarks>
    /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx">Path Finding Using A* Part THree</a>
    /// <typeparam name="T"></typeparam>
    public class ImmutableStack<T> : IEnumerable<T> {
        /// <summary>Construct a new empty instance.</summary>
        public ImmutableStack(T start) : this(start, null) {}

        /// <summary>Construct a new instance by Push-ing <paramref name="item"/> onto <paramref name="remainder"/>.</summary>
        private ImmutableStack(T item, ImmutableStack<T> remainder) {
            TopItem   = item;
            Remainder = remainder;
        }

        /// <summary>Gets the top item on the stack.</summary>
        public T                 TopItem      { get; }

        /// <summary>Gets the remainder of the stack.</summary>
        public ImmutableStack<T> Remainder    { get; }

        /// <summary>Returns a new ImmutableStack by adding <paramref name="item"/> to this stack.</summary>
        public ImmutableStack<T> Push(T item) => new ImmutableStack<T>(item, this);

        /// <summary>Returns the stackitems in order from top to bottom.</summary>
        public IEnumerator<T> GetEnumerator() {
            for (var p = this; p != null; p = p.Remainder) { yield return p.TopItem; }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
