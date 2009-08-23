// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.


using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
	public class CompareFail : CompareOperator<Cell> {
	    private static readonly IdentifierName failIdentifier = new IdentifierName("fail[");

	    public bool CanCompare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
	        return expected.Value.Text != null && failIdentifier.IsStartOf(expected.Value.Text) &&
	            expected.Value.Text.EndsWith("]");
	    }

	    public bool Compare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            //todo: use cellsubstring?
			string expectedValue = expected.Value.Text.Substring("fail[".Length, expected.Value.Text.Length - ("fail[".Length + 1));
			var newCell = new Parse("td", expectedValue, null, null);
            return !processor.Compare(actual, newCell);
	    }
	}
}