// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Data;
using fit;
using fit.Operators;
using fitSharp.Fit.Engine;

namespace fitlibrary {

	public class SubsetFixture: NamedCollectionFixtureBase {

        public SubsetFixture(object[] theArray): base(theArray) {}

        public SubsetFixture(ICollection theCollection): base(theCollection) {}

        public SubsetFixture(IEnumerator theEnumerator): base(theEnumerator) {}

        public SubsetFixture(DataTable theTable): base(theTable.Rows.GetEnumerator()) {}

        protected override ListMatchStrategy MatchStrategy {
            get {return new SubsetMatchStrategy(Processor, myHeaderRow);}
        }

        private class SubsetMatchStrategy: NamedMatchStrategy {
            public SubsetMatchStrategy(CellProcessor processor, Parse theHeaderRow): base(processor, theHeaderRow) {}
            public override bool IsOrdered {get { return false; }}
            public override bool SurplusAllowed {get {return true;}}

        }
    }
}
