// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class TimesCombinationSetUp: CombinationFixture {
    
        public void SetUp() {
            isSetUp = true;
        }

        public void TearDown() {
            throw new ApplicationException("tear down");
        }

        public int Combine(int x, int y) {
            if (!isSetUp)
                throw new ApplicationException("Not set up");
            return x * y;
        }

        public TimesCombinationSetUp DoIt() {
            return new TimesCombinationSetUp();
        }

        private bool isSetUp = false;
    }
}