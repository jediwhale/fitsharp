// FitNesse.NET
// Copyright © 2006-2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Data;
using fit;

namespace fitlibrary {

	public class SetFixture: NamedCollectionFixtureBase {

        public SetFixture(object[] theArray): base(theArray) {}

        public SetFixture(ICollection theCollection): base(theCollection) {}

        public SetFixture(IEnumerator theEnumerator): base(theEnumerator) {}

        public SetFixture(DataTable theTable): base(theTable.Rows.GetEnumerator()) {}

        protected override ListMatchStrategy MatchStrategy {
            get {
                return new SetMatchStrategy(myHeaderRow);
            }
        }

        private class SetMatchStrategy: NamedMatchStrategy {
            public SetMatchStrategy(Parse theHeaderRow): base(theHeaderRow) {}
            public override bool IsOrdered {get { return false; }}
            public override bool SurplusAllowed {get {return false;}}
        }
    }
}
