// Copyright © 2011 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Data;
using fit;
using fit.Operators;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace dbfit.fixture
{
    public class Query : NamedCollectionFixtureBase
    {
        private readonly bool isOrdered;
        private readonly bool isStandAlone;

        public Query(): base(new object[] {}) {
            isStandAlone = true;
        }

        public Query(IEnumerator enumerator, bool isOrdered): base(enumerator) {
            this.isOrdered = isOrdered;
        }

        public Query(DataTable queryTable, bool isOrdered): base(queryTable.Rows.GetEnumerator())
        {
            this.isOrdered = isOrdered;
        }

        public override void DoTable(Parse table)
        {
            if (isStandAlone) SetCollection(DatabaseTest.GetDataTable(
                Symbols,
                GetArgumentInput<String>(0),
                DbEnvironmentFactory.DefaultEnvironment).Rows.GetEnumerator());
            base.DoTable(table);
        }

        protected override ListMatchStrategy MatchStrategy {
            get { return new QueryMatchStrategy(Processor, myHeaderRow, isOrdered); }
        }

        private class QueryMatchStrategy: NamedMatchStrategy {
            private readonly bool isOrdered;
            public QueryMatchStrategy(CellProcessor processor, Parse theHeaderRow, bool isOrdered)
                : base(processor, theHeaderRow) {
                this.isOrdered = isOrdered;
            }
            public override bool IsOrdered {get { return isOrdered; }}
            public override bool SurplusAllowed {get {return false;}}

            public override bool CellMatches(TypedValue actualValue, Parse expectedCell, int columnNumber) {
                if (myHeaderRow.Parts.At(columnNumber).Text.EndsWith("?")) return true;
                if (expectedCell.Text.Length == 0) return true;
                return base.CellMatches(actualValue, expectedCell, columnNumber);
            }
        }
    }
}
