// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteDefault: ExecuteBase {
        public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
            return true;
        }

        public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
            switch (context.Command) {
                case ExecuteCommand.Input:
                    Input(context, parameters);
                    break;
                case ExecuteCommand.Check:
                    Check(context, parameters);
                    break;
                case ExecuteCommand.Compare:
                    return new TypedValue(Processor.Compare(context.Target.Value, parameters.Cells));

                case ExecuteCommand.Invoke:
                    return Invoke(context, parameters);

                default:
                    throw new ArgumentException(string.Format("Unrecognized operation '{0}'", context.Command));
            }
            return TypedValue.Void;
        }

        private  void Input(ExecuteContext context, ExecuteParameters parameters) {
            InvokeWithThrow(context.SystemUnderTest, GetMemberName(parameters.Members),
                             new TreeList<Cell>().AddBranch(parameters.Cells));
        }

        private  void Check(ExecuteContext context, ExecuteParameters parameters) {
            try {
                TypedValue actual = GetTypedActual(context, parameters);
                if (Processor.Compare(actual, parameters.Cells)) {
                    Processor.TestStatus.MarkRight(parameters.Cell);
                }
                else {
                    Processor.TestStatus.MarkWrong(parameters.Cell, actual.ValueString);
                }
            }
            catch (IgnoredException) {}
        }

        private TypedValue Invoke(ExecuteContext context, ExecuteParameters parameters) {
            var beforeCounts = new TestCounts(Processor.TestStatus.Counts);
            TypedValue target = context.Target.Value;
            var targetObjectProvider = target.Value as TargetObjectProvider;
            var name = ParseTree<MemberName>(parameters.Members);
            TypedValue result = Processor.Invoke(
                    targetObjectProvider != null ? new TypedValue(targetObjectProvider.GetTargetObject()) : target,
                    name.ToString(), parameters.Parameters);
            if (parameters.Cells != null && !string.IsNullOrEmpty(Processor.TestStatus.LastAction)) {
                parameters.Cell.SetAttribute(CellAttributes.ExtensionKey, Processor.TestStatus.LastAction);
                string style = Processor.TestStatus.Counts.Subtract(beforeCounts).Style;
                if (!string.IsNullOrEmpty(style)) parameters.Cell.SetAttribute(CellAttributes.StatusKey, style);
                Processor.TestStatus.LastAction = null;
            }
            return result;
        }
    }
}