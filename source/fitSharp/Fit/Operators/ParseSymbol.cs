// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseSymbol: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return parameters.Value != null
                && parameters.Value.Content.StartsWith("<<")
                && !parameters.Value.Content.Contains(",");
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var symbols = Processor.Get<Symbols>();
            var symbol = parameters.Value.Content.Substring(2);
            var result = symbols.HasValue(symbol) ? MakeTypedValue(symbols.GetValue(symbol), type) : new TypedValue(null, type);
            parameters.Value.SetAttribute(
                CellAttribute.InformationSuffix,
                result.Value == null ? "null" : result.Value.ToString()); //todo: dry
            return result;
        }

        TypedValue MakeTypedValue(object value, Type type) {
            return value == null || type.IsInstanceOfType(value)
                ? new TypedValue(value, type)
                : Processor.ParseString(type, value.ToString());
        }
    }
}
