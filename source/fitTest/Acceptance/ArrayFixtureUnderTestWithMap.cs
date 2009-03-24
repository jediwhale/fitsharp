// Copyright (c) 2004 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

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