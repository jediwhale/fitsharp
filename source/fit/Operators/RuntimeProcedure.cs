// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class RuntimeProcedure: CellOperator, RuntimeOperator<Cell>
    {
        public bool CanCreate(string memberName, Tree<Cell> parameters) { return false; }

        public TypedValue Create(string memberName, Tree<Cell> parameters) { return TypedValue.Void; }

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return Processor.Contains(new Procedure(memberName));
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var procedure = Processor.Load(new Procedure(memberName));
            return Invoke((Parse)procedure.Instance, instance, parameters);
        }

        private TypedValue Invoke(Parse procedure, TypedValue target, Tree<Cell> parameterValues) {
            var doFixture = new CellTree(new CellTree("dofixture"));
            var fixture = Processor.Parse(typeof (Interpreter), target, doFixture);

            Parse body = DeepCopy(procedure, procedure.Parts, parameterValues);
            body.More = null;
            body.Parts = body.Parts.More;

            TypedValue result = Processor.Execute(fixture, new CellTree((Tree<Cell>)body));

            Processor.TestStatus.LastAction = Processor.Parse(typeof(StoryTestString), TypedValue.Void, body).ValueString;
            return result;
        }

        private static Parse DeepCopy(Parse source, Tree<Cell> parameterNames, Tree<Cell> parameterValues) {
            int i = 2;
            foreach (Tree<Cell> parameterValue in parameterValues.Branches) {
                if (parameterNames.Branches[i].Value.Text == source.Value.Text) {
                    return ((Parse) parameterValue).DeepCopy();
                }
                i += 2;
            }
            return new Parse(source.Tag, source.End, source.Leader, source.Body, (source.Parts == null ? null : DeepCopy(source.Parts, parameterNames, parameterValues))) {
                Trailer = source.Trailer,
                More = (source.More == null ? null : DeepCopy(source.More, parameterNames, parameterValues))
            };
        }
    }
}