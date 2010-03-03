// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class ParseByteArray: CellOperator, ParseOperator<Cell> {
        private readonly IdentifierName prefix = new IdentifierName("0x");

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof (byte[]) && prefix.IsStartOf(parameters.Value.Text);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            string input = parameters.Value.Text;
			input = input.Replace(" ", string.Empty);
			input = input.Substring(2);
			var result = new byte[input.Length / 2];
			for (int i = 0; i < input.Length; i += 2) {
			    string currentByte = input.Substring(i, 2);
			    result[i/2] = Byte.Parse(currentByte, System.Globalization.NumberStyles.HexNumber);
			}
            return new TypedValue(result);
        }
    }
}
