// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

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