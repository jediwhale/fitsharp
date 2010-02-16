// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseSymbol: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return parameters.Value != null && parameters.Value.Text.StartsWith("<<");
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var symbol = new Symbol(parameters.Value.Text.Substring(2));
            var result = new TypedValue(Processor.Contains(symbol) ? Processor.Load(symbol).Instance : null, type);
            parameters.Value.AddToAttribute(
                CellAttribute.InformationSuffix,
                result.Value == null ? "null" : result.Value.ToString(),
                CellAttributes.SuffixFormat);
            return result;
        }
    }
}
