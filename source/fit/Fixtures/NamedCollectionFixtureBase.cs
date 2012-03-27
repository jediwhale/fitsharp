// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System.Collections;
using System.Collections.Generic;
using fitSharp.Fit.Exception;

namespace fitlibrary {

	public abstract class NamedCollectionFixtureBase: CollectionFixtureBase {
        protected NamedCollectionFixtureBase(): base() {}

	    protected NamedCollectionFixtureBase(IEnumerable<object> theArray): base(theArray) {}

	    protected NamedCollectionFixtureBase(IEnumerable theCollection): base(theCollection) {}

	    protected NamedCollectionFixtureBase(IEnumerator theEnumerator): base(theEnumerator) {}

        public override void DoTable(Parse table) {
            var rows = table.Parts.More;
            if (rows == null) throw new TableStructureException("Header row missing.");
            myHeaderRow = rows;
            CompareRows(table, 1);
        }

        protected Parse myHeaderRow;
    }

}
