// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureSetUpWithException: CalculateFixture {
        public void setUp() {
            throw new ApplicationException("setUp exception.");
        }
        public int resultA(int a) { 
            return a+1;
        }
    }
}