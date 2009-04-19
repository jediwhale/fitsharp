// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System.Collections;
using fitlibrary.exception;

namespace fitlibrary {

	public abstract class NamedCollectionFixtureBase: CollectionFixtureBase {

        public NamedCollectionFixtureBase(object[] theArray): base(theArray) {}

        public NamedCollectionFixtureBase(ICollection theCollection): base(theCollection) {}

        public NamedCollectionFixtureBase(IEnumerator theEnumerator): base(theEnumerator) {}

        public override void DoRows(Parse theRows) {
            if (theRows == null) throw new TableStructureException("Header row missing.");
            myHeaderRow = theRows;
            CompareRows(theRows);
        }

        protected Parse myHeaderRow;
    }

}
