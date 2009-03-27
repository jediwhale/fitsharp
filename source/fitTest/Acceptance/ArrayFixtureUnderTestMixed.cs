// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class ArrayFixtureUnderTestMixed: ArrayFixture {

        public ArrayFixtureUnderTestMixed(): base(MakeMixedList()) {}

        private static ArrayList MakeMixedList() {
            ArrayList result = new ArrayList();
            result.Add(MakeMap(1, "one"));
            result.Add(new MockCollection(1,"two"));
            result.Add(MakeMap(2, "two"));
            return result;
        }

        private static Hashtable MakeMap(int thePlus, string theAmpersand) {
            Hashtable result = new Hashtable();
            result.Add("+", thePlus);
            result.Add("&", theAmpersand);
            return result;
        }
    }
}