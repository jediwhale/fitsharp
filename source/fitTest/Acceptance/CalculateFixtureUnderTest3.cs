// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureUnderTest3: CalculateFixture {
        public CalculateFixtureUnderTest3() {
            RepeatString = "";
        }
        public int plusAB(int a, int b) {
            return a + b;
        }
    }
}