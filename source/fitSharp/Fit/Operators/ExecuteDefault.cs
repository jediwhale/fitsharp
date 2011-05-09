// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteDefault: InvokeCommandBase {
        public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
            return true;
        }

        public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
            switch (context.Command) {
                case ExecuteCommand.Check:
                    Check(context, parameters);
                    break;

                case ExecuteCommand.Invoke:
                    return Invoke(context, parameters);

                default:
                    throw new ArgumentException(string.Format("Unrecognized operation '{0}'", context.Command));
            }
            return TypedValue.Void;
        }

        void Check(ExecuteContext context, ExecuteParameters parameters) {
            try {
                TypedValue actual = GetTypedActual(context, parameters);
                if (Processor.Compare(actual, parameters.Cells)) {
                    Processor.TestStatus.MarkRight(parameters.Cell);
                }
                else {
                    var actualCell = Processor.Compose(actual);
                    Processor.TestStatus.MarkWrong(parameters.Cell, actualCell.Value.Text);
                }
            }
            catch (IgnoredException) {}
            MarkCellWithLastResults(parameters, p => {});
        }

        TypedValue Invoke(ExecuteContext context, ExecuteParameters parameters) {
            var beforeCounts = new TestCounts(Processor.TestStatus.Counts);
            TypedValue target = context.Target.Value;
            var targetObjectProvider = target.Value as TargetObjectProvider;
            string name = GetMemberName(parameters.Members);
            TypedValue result = Processor.Invoke(
                    targetObjectProvider != null ? new TypedValue(targetObjectProvider.GetTargetObject()) : target,
                    name, parameters.Parameters);
            MarkCellWithLastResults(parameters, p => MarkCellWithCounts(p, beforeCounts));
            return result;
        }

        void MarkCellWithLastResults(ExecuteParameters parameters, Action<ExecuteParameters> markWithCounts) {
            if (parameters.Cells != null && !string.IsNullOrEmpty(Processor.TestStatus.LastAction)) {
                parameters.Cell.SetAttribute(CellAttribute.Folded, Processor.TestStatus.LastAction);
                markWithCounts(parameters);
            }
            Processor.TestStatus.LastAction = null;
        }

        void MarkCellWithCounts(ExecuteParameters parameters, TestCounts beforeCounts) {
            string style = Processor.TestStatus.Counts.Subtract(beforeCounts).Style;
            if (!string.IsNullOrEmpty(style) && string.IsNullOrEmpty(parameters.Cell.GetAttribute(CellAttribute.Status))) {
                parameters.Cell.SetAttribute(CellAttribute.Status, style);
            }
        }
    }
}
