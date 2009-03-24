// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class SetFixtureUnderTest2: SetFixture {
        public SetFixtureUnderTest2():
            base(new Object[]{
                new MockCollection(1,"one"),
                new MockCollection(1,"two"),
                new MockCollection(1,"two"),
                new Some()})
        {}
    }
}