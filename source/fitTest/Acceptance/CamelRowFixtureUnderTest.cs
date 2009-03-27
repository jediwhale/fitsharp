// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

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