// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Collections.Generic;
using fit;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitlibrary {

    public class UnnamedCollectionFixtureBase: CollectionFixtureBase {

        public UnnamedCollectionFixtureBase(IEnumerator theEnumerator): base(theEnumerator) {}

        public UnnamedCollectionFixtureBase(IEnumerable<object[]> theActualValues): base(theActualValues) {}

        public override void DoTable(Parse theTable) {
            CompareRows(theTable, 0);
        }

        protected override ListMatchStrategy MatchStrategy {
            get {return new UnnamedCollectionMatchStrategy(Processor);}
        }

        private class UnnamedCollectionMatchStrategy: CellMatcher, ListMatchStrategy {
            public UnnamedCollectionMatchStrategy(CellProcessor processor) : base(processor) {}
            public bool IsOrdered {get { return true; }}
            public bool SurplusAllowed {get {return false;}}
            public TypedValue[] ActualValues(object theActualRow) {
                var actuals = (object[]) theActualRow;
                var result = new TypedValue[actuals.Length];
                for (int i = 0; i < actuals.Length; i++) result[i] = new TypedValue(actuals[i]);
                return result;
            }
            public bool IsExpectedSize(int expectedSize, object theActualRow) {
                return expectedSize == ((object[])theActualRow).Length;
            }
            public bool FinalCheck(TestStatus testStatus) {return true;}
        }
    }
}
