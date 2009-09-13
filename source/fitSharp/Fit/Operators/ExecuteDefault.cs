// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

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
            Processor.Invoke(parameters.SystemUnderTest, parameters.GetMemberName(Processor),
                             new TreeList<Cell>().AddBranch(parameters.Cells));
        }

        private  void Check(ExecuteParameters parameters) {
            try {
                TypedValue actual = parameters.GetTypedActual(Processor);
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
            var name = Processor.ParseTree<MemberName>(parameters.Members);
            return Processor.TryInvoke(targetObjectProvider != null ? new TypedValue(targetObjectProvider.GetTargetObject()) : target, name.ToString(), parameters.Parameters);
        }
    }
}