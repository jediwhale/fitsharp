// Copyright © 2012 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fit;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public abstract class CollectionFixtureBase: Fixture {
	    protected CollectionFixtureBase() {
            myArray = new List<object>();
        }

	    protected CollectionFixtureBase(IEnumerable<object> theArray) {
            myArray = new List<object>(theArray);
        }

	    protected CollectionFixtureBase(IEnumerable theCollection) {
	        myArray = theCollection.Cast<object>().ToList();
        }

	    protected CollectionFixtureBase(IEnumerator theEnumerator) {
	        SetCollection(theEnumerator);
	    }

	    protected void SetCollection(IEnumerator enumerator) {
	        var collection  = new List<object>();
	        while (enumerator.MoveNext()) {
	            collection.Add(enumerator.Current);
	        }
            SetSystemUnderTest(collection);
	    }

	    protected abstract ListMatchStrategy MatchStrategy {get;}

        protected void CompareRows(Tree<Cell> table, int rowsToSkip) {
            var matcher = new ListMatcher(Processor, MatchStrategy);
            matcher.MarkCell(myArray, table, rowsToSkip);
        }

	    protected IEnumerable<object> myArray {
	        get {
                if (SystemUnderTest == null) return null;
                var genericResult = SystemUnderTest as IEnumerable<object>;
                if (genericResult != null) return genericResult;
	            var result = SystemUnderTest as IEnumerable;
	            if (result != null) return result.Cast<object>().ToList();
                throw new ApplicationException(string.Format("{0} is not IEnumerable", SystemUnderTest.GetType().FullName));
	        }
            set { SetSystemUnderTest(value);  }
	    }
	}
}
