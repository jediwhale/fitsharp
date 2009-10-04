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
            return Invoke(procedure.Instance, instance, parameters);
        }

        private TypedValue Invoke(Tree<Cell> procedure, TypedValue target, Tree<Cell> parameters) {
            var doFixture = new CellTree(new CellTree("dofixture"));
            var fixture = Processor.Parse(typeof (Interpreter), target, doFixture);

            var body = ((Parse)procedure).DeepCopy();
            body.More = null;
            foreach (Tree<Cell> leaf in body.Parts.More.Leaves()) {
                int i = 2;
                foreach (Tree<Cell> parameterValue in parameters.Branches) {
                    if (body.Parts.Branches[i].Value.Text == leaf.Value.Text) {
                        ((Parse) leaf.Value).SetOriginalBody(parameterValue.Value.Text);
                    }
                    i += 2;
                }
            }
            body.Parts = body.Parts.More;

            TypedValue result = Processor.Execute(fixture, new CellTree((Tree<Cell>)body));

            Processor.TestStatus.LastAction = Processor.Parse(typeof(StoryTestString), TypedValue.Void, body).ValueString;
            return result;
        }
    }
}