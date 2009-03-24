// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

namespace fit.Test.Acceptance {
    public class MockCollection {
        public int plus = 0;
        public string ampersand;
	
        public MockCollection(int plus, string ampersand) {
            this.plus = plus;
            this.ampersand = ampersand;
        }
        public int Prop {get {return plus;}}
    }
}