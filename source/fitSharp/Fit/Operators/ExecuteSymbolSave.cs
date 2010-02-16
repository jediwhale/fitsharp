// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteSymbolSave : ExecuteBase {
        public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
            return context.Command == ExecuteCommand.Check
                && parameters.Cell.Text.StartsWith(">>");
        }

        public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
            object value = GetActual(context, parameters);
            var symbol = new Symbol(parameters.Cell.Text.Substring(2), value);
            Processor.Store(symbol);

            parameters.Cell.AddToAttribute(CellAttribute.InformationSuffix, value == null ? "null" : value.ToString(), CellAttributes.SuffixFormat);

            return TypedValue.Void;
        }
    }
}
