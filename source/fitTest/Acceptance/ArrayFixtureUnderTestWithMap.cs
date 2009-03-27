// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class ArrayFixtureUnderTestWithMap: ArrayFixture {

        public ArrayFixtureUnderTestWithMap(): base(MakeMapList()) {}

        private static ArrayList MakeMapList() {
            ArrayList result = new ArrayList();
            result.Add(MakeMap(1, "one", "ONE"));
            result.Add(MakeMap(1, "two", "TWO"));
            result.Add(MakeMap(2, "two", "TWO"));
            return result;
        }

        private static Hashtable MakeMap(int thePlus, string theAmpersand, string theBig) {
            Hashtable result = new Hashtable();
            result.Add("+", thePlus);
            result.Add("&", theAmpersand);
            result.Add("BIG", theBig);
            return result;
        }
    }
}