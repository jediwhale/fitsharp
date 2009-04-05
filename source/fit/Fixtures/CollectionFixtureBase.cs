// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fit;
using fit.Operators;

namespace fitlibrary {

	public abstract class CollectionFixtureBase: Fixture {

        public CollectionFixtureBase() {
            myArray = new ArrayList();
        }

        public CollectionFixtureBase(object[] theArray) {
            myArray = new ArrayList(theArray);
        }

        public CollectionFixtureBase(ICollection theCollection) {
            myArray = new ArrayList(theCollection);
        }

        public CollectionFixtureBase(IEnumerator theEnumerator): this() {
            while (theEnumerator.MoveNext()) {
                myArray.Add(theEnumerator.Current);
            }
        }

        public CollectionFixtureBase(object[][] theGrid): this() {
            for (int i = 0; i < theGrid.Length; i++) {
                myArray.Add(theGrid[i]);
            }
        }

        protected abstract ListMatchStrategy MatchStrategy {get;}

        protected void CompareRows(Parse theTableRows, Parse theRowsToCompare) {
            ListMatcher matcher = new ListMatcher(Processor, MatchStrategy);
            matcher.MarkCell(this, myArray, theTableRows, theRowsToCompare);
        }

        protected ArrayList myArray;
	}
}
