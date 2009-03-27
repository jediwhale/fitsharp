// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class MapFixture: DoFixture {
        public SetFixture Map {
            get {
                Hashtable theMap = new Hashtable();
                theMap.Add("a", "b");     
                theMap.Add("c", "d");     
                return new SetFixture(theMap);
            }
        }
        public SubsetFixture SubsetMap {
            get {
                Hashtable theMap = new Hashtable();
                theMap.Add("a", "b");     
                theMap.Add("c", "d");     
                return new SubsetFixture(theMap);
            }
        }
    }
}