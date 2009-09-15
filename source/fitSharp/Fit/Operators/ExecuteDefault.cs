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
        public override bool CanExecute(ExecuteParameters parameters) {
            return true;
        }

        public override TypedValue Execute(ExecuteParameters parameters) {
            switch (parameters.Verb) {
                case ExecuteParameters.Input:
                    Input(parameters);
                    break;

                case ExecuteParameters.Check:
                    Check(parameters);
                    break;

                case ExecuteParameters.Compare:
                    return new TypedValue(Processor.Compare(parameters.Target, parameters.Cells));

                case ExecuteParameters.Invoke:
                    return Invoke(parameters);

                default:
                    throw new ArgumentException(string.Format("Unrecognized operation '{0}'", parameters.Verb));
            }
            return TypedValue.Void;
        }

        private  void Input(ExecuteParameters parameters) {
            InvokeWithThrow(parameters.SystemUnderTest, GetMemberName(parameters.Members),
                             new TreeList<Cell>().AddBranch(parameters.Cells));
        }

        private  void Check(ExecuteParameters parameters) {
            try {
                TypedValue actual = parameters.GetTypedActual(this);
                if (Processor.Compare(actual, parameters.Cells)) {
                    parameters.TestStatus.MarkRight(parameters.Cell);
                }
                else {
                    parameters.TestStatus.MarkWrong(parameters.Cell, actual.ValueString);
                }
            }
            catch (IgnoredException) {}
        }

        private TypedValue Invoke(ExecuteParameters parameters) {
            TypedValue target = parameters.Target;
            var targetObjectProvider = target.Value as TargetObjectProvider;
            var name = ParseTree<MemberName>(parameters.Members);
            return Processor.Invoke(targetObjectProvider != null ? new TypedValue(targetObjectProvider.GetTargetObject()) : target, name.ToString(), parameters.Parameters);
        }
    }
}