// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class SetUpFixtureUnderTest2: SetUpFixture {
        private bool setup = false;
        protected override void SetUp() {
            setup = true; 
        }
        public void aPercent(int a, int b) {
            if (!setup)
                throw new ApplicationException("no setup");
        }
        protected override void TearDown() {
            throw new ApplicationException("teardown");
        }
    }
}