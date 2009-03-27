// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureSetUp: CalculateFixture {
    
        public void setUp() {
            IAmSetUp = true;
        }
        public int resultA(int a) { 
            if (IAmSetUp)
                return a+1;
            return 0;
        }
        private bool IAmSetUp = false;
    }
}