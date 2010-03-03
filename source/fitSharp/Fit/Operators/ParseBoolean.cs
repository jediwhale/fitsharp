// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseBoolean: CellOperator, ParseOperator<Cell> {
	    private static readonly IdentifierName ourTrueIdentifier = new IdentifierName("true");
	    private static readonly IdentifierName ourYesIdentifier = new IdentifierName("yes");
	    private static readonly IdentifierName ourYIdentifier = new IdentifierName("y");
	    private static readonly IdentifierName ourFalseIdentifier = new IdentifierName("false");
	    private static readonly IdentifierName ourNoIdentifier = new IdentifierName("no");
	    private static readonly IdentifierName ourNIdentifier = new IdentifierName("n");

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(bool);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return new TypedValue(ImpliesTrue(parameters.Value.Text)
                       ? true
                       : ImpliesFalse(parameters.Value.Text)
                             ? false
                             : bool.Parse(parameters.Value.Text));
        }

		private static bool ImpliesTrue(string possibleTrue) {
		    return ourYIdentifier.Equals(possibleTrue) || ourYesIdentifier.Equals(possibleTrue) || ourTrueIdentifier.Equals(possibleTrue);
		}

		private static bool ImpliesFalse(string possibleFalse) {
		    return
		        ourNIdentifier.Equals(possibleFalse) || ourNoIdentifier.Equals(possibleFalse) || ourFalseIdentifier.Equals(possibleFalse);
		}
    }
}
