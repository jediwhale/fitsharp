// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
	public class CompareFail : CellOperator, CompareOperator<Cell> {
	    private static readonly IdentifierName failIdentifier = new IdentifierName("fail[");

	    public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
	        return expected.Value.Text != null && failIdentifier.IsStartOf(expected.Value.Text) &&
	            expected.Value.Text.EndsWith("]");
	    }

	    public bool Compare(TypedValue actual, Tree<Cell> expected) {
			string expectedText = expected.Value.Text.Substring("fail[".Length, expected.Value.Text.Length - ("fail[".Length + 1));
            var rest = new TreeLeaf<Cell>(new CellSubstring(expected.Value, expectedText));
            return !Processor.Compare(actual, rest);
	    }
	}
}
