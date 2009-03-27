// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

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