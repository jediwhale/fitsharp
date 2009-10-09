// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace fit.Test.Double {
    public class SampleRowFixture: RowFixture {
        public override object[] Query() {
            return new TwoNames[] {new TwoNames("too short", "<stuff>"), new TwoNames("too", "too")};
        }

        public override Type GetTargetClass() {
            return typeof(TwoNames);
        }

        private class TwoNames {
            public string name1;
            public string name2;
            public TwoNames(string theFirst, string theSecond) {
                name1 = theFirst;
                name2 = theSecond;
            }
        }
    }
}