// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using System;

namespace fit.Test.Acceptance {
    public class CamelRowFixtureUnderTest: RowFixture {

        public override object[] Query() {
            return new MockCollection[]{
                new MockCollection(1,"one"),
                new MockCollection(1,"two"),
                new MockCollection(2,"two")};
        }
        public override Type GetTargetClass() {
            return typeof(MockCollection);
        }
    }
}