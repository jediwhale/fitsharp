// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using System.Collections;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class ArrayFixtureUnderTest3: ArrayFixture {
        public ArrayFixtureUnderTest3(): base(new ArrayList()) {}
    }
}