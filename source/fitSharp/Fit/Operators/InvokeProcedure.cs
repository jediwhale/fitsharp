// Copyright © 2016 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Linq;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeProcedure: CellOperator, InvokeOperator<Cell>
    {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return Processor.Get<Procedures>().HasValue(memberName.Name);
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            var procedure = Processor.Get<Procedures>().GetValue(memberName.Name);
            return Invoke((Tree<Cell>)procedure, instance, parameters);
        }

        private TypedValue Invoke(Tree<Cell> procedure, TypedValue target, Tree<Cell> parameterValues) {
            var copy = new DeepCopy(Processor);
            var parameters = new Parameters(procedure.Branches[0], parameterValues, copy);
            var body = procedure.Branches[1].Branches[0].IsLeaf
                ? new CellTree(copy.Make(procedure,
                    source => source.Branches.Skip(1),
                    parameters.Substitute))
                : new CellTree(
                    procedure.Branches[1].Branches[0].Branches.Select(branch => copy.Make(branch, parameters.Substitute)));
            body.ValueAt(0).ClearAttribute(CellAttribute.Leader);

            Processor.CallStack.Push();
            var fixture = new DefaultFlowInterpreter(target.Value);
            ExecuteProcedure(fixture, body);
            Processor.TestStatus.LastAction = Processor.ParseTree(typeof(StoryTestString), body).ValueString;
            return Processor.CallStack.PopReturn();
        }

        void ExecuteProcedure(FlowInterpreter flowInterpreter, Tree<Cell> body) {
            foreach (var table in body.Branches) {
                new InterpretFlow(Processor, flowInterpreter).DoTableFlow(table, 0);
            }
        }

        class Parameters {
            private readonly Tree<Cell> names;
            private readonly Tree<Cell> values;
            private readonly DeepCopy copy;

            public Parameters(Tree<Cell> names, Tree<Cell> values, DeepCopy copy) {
                this.names = names;
                this.values = values;
                this.copy = copy;
            }

            public Tree<Cell> Substitute(Tree<Cell> source) {
                var i = 2;
                foreach (var parameterValue in values.Branches) {
                    if (source.Value != null && names.ValueAt(i).Text == source.Value.Text) {
                        return copy.Make(parameterValue);
                    }
                    i += 2;
                }
                return null;
            }
        }
    }
}
