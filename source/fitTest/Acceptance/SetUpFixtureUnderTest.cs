// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class SetUpFixtureUnderTest: SetUpFixture {
        public void aB(int a, int b) {
            if (a < 0)
                throw new ApplicationException("test");
        }
    }
}