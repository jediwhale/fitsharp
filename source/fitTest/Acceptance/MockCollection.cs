// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

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