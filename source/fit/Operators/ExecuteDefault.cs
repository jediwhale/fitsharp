// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Engine;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ExecuteDefault: ExecuteBase {
        public override bool TryExecute(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result) {
            switch (parameters.Verb) {
                case ExecuteParameters.Input:
                    Input(processor, parameters);
                    break;

                case ExecuteParameters.Check:
                    Check(processor, parameters);
                    break;

                case ExecuteParameters.Compare:
                    result = new TypedValue(processor.Compare(parameters.Target, parameters.Cells));
                    break;

                case ExecuteParameters.Invoke:
                    result = Invoke(processor, parameters);
                    break;

                default:
                    throw new ArgumentException(string.Format("Unrecognized operation '{0}'", parameters.Verb));
            }
            return true;
        }

        private static void Input(Processor<Cell> processor, ExecuteParameters parameters) {
            processor.Invoke(parameters.SystemUnderTest, parameters.GetMemberName(processor),
                             new TreeList<Cell>().AddBranch(parameters.Cells));
        }

        private static void Check(Processor<Cell> processor, ExecuteParameters parameters) {
            try {
                TypedValue actual = parameters.GetTypedActual(processor);
                if (processor.Compare(actual, parameters.Cells)) {
                    parameters.TestStatus.MarkRight(parameters.Cell);
                }
                else {
                    parameters.TestStatus.MarkWrong(parameters.Cell, actual.ValueString);
                }
            }
            catch (IgnoredException) {}
        }

        private static TypedValue Invoke(Processor<Cell> processor, ExecuteParameters parameters) {
            TypedValue target = parameters.Target;
            var targetObjectProvider = target.Value as TargetObjectProvider;
            var name = processor.ParseTree<MemberName>(parameters.Members);
            return processor.TryInvoke(targetObjectProvider != null ? new TypedValue(targetObjectProvider.GetTargetObject()) : target, name.ToString(), parameters.Parameters);
        }
    }
}
