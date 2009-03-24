// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureUnderTest2: CalculateFixture {

        public CalculateFixtureUnderTest2() {
            RepeatString = "\"";
        }
        public int plusAB(int a, int b) {
            return a + b;
        }
        public string exceptionMethod() {
            throw new ApplicationException("Expected exception");
        }
    }
}