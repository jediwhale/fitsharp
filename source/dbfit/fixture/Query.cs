// Copyright © 2011 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Data.Common;
using System.Data;
using dbfit.util;
using fit;
using fit.Operators;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace dbfit.fixture
{
    public class Query : NamedCollectionFixtureBase
    {
        private readonly bool isOrdered;

        public Query(): base(new object[] {}) {
            myArray = null;
        }

        public Query(IDbEnvironment environment, String query, bool isOrdered): base(GetDataTable(query, environment).Rows.GetEnumerator())
        {
            this.isOrdered = isOrdered;
        }

        public Query(IDbEnvironment environment, String query, bool isOrdered,int rsNo)
            : base(GetDataTable(query, environment,rsNo).Rows.GetEnumerator())
        {
            this.isOrdered = isOrdered;
        }

        public Query(DataTable queryTable, bool isOrdered): base(queryTable.Rows.GetEnumerator())
        {
            this.isOrdered = isOrdered;
        }

        public override void DoTable(Parse table)
        {
            if (myArray == null) SetCollection(GetDataTable(GetArgumentInput<String>(0), DbEnvironmentFactory.DefaultEnvironment).Rows.GetEnumerator());
            base.DoTable(table);
        }

        public static DataTable GetDataTable(String query,IDbEnvironment environment, int rsNo)
        {
            DbCommand dc = environment.CreateCommand(query, CommandType.Text);
            if (Options.ShouldBindSymbols())
                environment.BindFixtureSymbols(dc);

            DbDataAdapter oap = environment.DbProviderFactory.CreateDataAdapter();
            oap.SelectCommand = dc;
            var ds = new DataSet();
            oap.Fill(ds);
            dc.Dispose();
            return ds.Tables[rsNo - 1];
        }

        public static DataTable GetDataTable(String query, IDbEnvironment environment)
        {
            return GetDataTable(query, environment, 1);
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
