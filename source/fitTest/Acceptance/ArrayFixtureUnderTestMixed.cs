// Copyright (c) 2004 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

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