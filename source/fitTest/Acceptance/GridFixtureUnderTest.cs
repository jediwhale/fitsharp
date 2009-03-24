// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

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