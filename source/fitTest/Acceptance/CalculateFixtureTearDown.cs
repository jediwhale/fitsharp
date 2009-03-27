// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureTearDown: CalculateFixture {
        protected void tearDown() {
            throw new ApplicationException("TearDown Worked.");
        }
        public int resultA(int a) { 
            throw new ApplicationException("ex"+a);
        }
    }
}