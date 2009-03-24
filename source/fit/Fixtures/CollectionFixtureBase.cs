// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using System.Collections;
using fit;

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
            ListMatcher matcher = new ListMatcher(MatchStrategy);
            matcher.MarkCell(this, myArray, theTableRows, theRowsToCompare);
        }

        protected ArrayList myArray;
	}
}
