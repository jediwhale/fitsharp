// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitlibrary.tree;

namespace fit.Test.Acceptance {
    public class GridFixtureUnderTest: DoFixture {

        public Fixture Empty() {
            return new GridFixture(new object[][]{});      
        }

        public Fixture Strings() {
            return new GridFixture(new object[][]{new object[]{"a", "b"}, new object[]{"c", "d"}});      
        }

        public Fixture Ints() {
            return new GridFixture(new object[][]{new object[]{1, 2}, new object[]{3, 4}});      
        }
        public Fixture trees() {
            return new GridFixture(new ListTree[][] {
                new ListTree[] {
                    new ListTree("a"),
                    new ListTree(string.Empty, new ListTree[]{new ListTree("a")})
                },
                new ListTree[] {
                    new ListTree(string.Empty, new ListTree[]{new ListTree("BB")}),
                    new ListTree(string.Empty, new ListTree[]{new ListTree("a"), new ListTree("BB")})
                }
            });
        }
    }
}