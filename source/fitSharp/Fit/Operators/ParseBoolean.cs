using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseBoolean: ParseOperator<Cell> {
	    private static readonly IdentifierName ourTrueIdentifier = new IdentifierName("true");
	    private static readonly IdentifierName ourYesIdentifier = new IdentifierName("yes");
	    private static readonly IdentifierName ourYIdentifier = new IdentifierName("y");
	    private static readonly IdentifierName ourFalseIdentifier = new IdentifierName("false");
	    private static readonly IdentifierName ourNoIdentifier = new IdentifierName("no");
	    private static readonly IdentifierName ourNIdentifier = new IdentifierName("n");

        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (type != typeof(bool)) return false;
            result = new TypedValue(ImpliesTrue(parameters.Value.Text)
                       ? true
                       : ImpliesFalse(parameters.Value.Text)
                             ? false
                             : bool.Parse(parameters.Value.Text));
            return true;
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
