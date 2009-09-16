// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteParameters {

        public const string Check = "check";
        public const string Input = "input";
        public const string Compare = "compare";
        public const string Invoke = "invoke";

        private readonly Tree<Cell> tree;
        private readonly ExecuteContext context;

        public TypedValue SystemUnderTest { get { return context.SystemUnderTest; } }
        public TypedValue Target { get { return context.Target.Value; } }

        public string Verb { get { return tree.Branches[0].Value.Text; } }
        public Cell Cell { get { return tree.Branches[1].Value; } }
        public Tree<Cell> Cells { get { return tree.Branches[1]; } }
        public Tree<Cell> Members { get { return tree.Branches[2]; } }
        public Cell Member { get { return tree.Branches[2].Value; } }
        public Tree<Cell> Parameters { get { return tree.Branches[3]; } }

        public static Tree<Cell> MakeCompare(Tree<Cell> parameters) {
            return new TreeList<Cell>()
                .AddBranch(new StringCell(Compare))
                .AddBranch(parameters);
        }

        public static Tree<Cell> MakeCheck(Tree<Cell> member, Tree<Cell> parameters, Tree<Cell> expectedCell) {
            return new TreeList<Cell>()
                .AddBranch(new StringCell(Check))
                .AddBranch(expectedCell)
                .AddBranchValue(member)
                .AddBranch(parameters);
        }

        public static Tree<Cell> MakeCheck( Tree<Cell> expectedCell) {
            return new TreeList<Cell>()
                .AddBranch(new StringCell(Check))
                .AddBranch(expectedCell);
        }

        public static Tree<Cell> MakeInput(Tree<Cell> member, Tree<Cell> parameters) {
            return new TreeList<Cell>()
                .AddBranch(new StringCell(Input))
                .AddBranch(parameters)
                .AddBranchValue(member);
        }

        public static Tree<Cell> MakeInvoke(Tree<Cell> member, Tree<Cell> parameters) {
            return new TreeList<Cell>()
                .AddBranch(new StringCell(Invoke))
                .AddBranch(null)
                .AddBranch(member)
                .AddBranch(parameters);
        }

        public ExecuteParameters(ExecuteContext context, Tree<Cell> tree) {
            this.context = context;
            this.tree = tree;
        }

        public TypedValue GetTypedActual(CellOperator cellOperator) {
            if (!context.Target.HasValue) {
                try {
                    TypedValue actualResult = cellOperator.InvokeWithThrow(SystemUnderTest, cellOperator.GetMemberName(Members), Parameters);
                    context.Target = actualResult;
                }
                catch (ParseException<Cell> e) {
                    cellOperator.Processor.TestStatus.MarkException(e.Subject, e);
                    throw new IgnoredException();
                }
            }
            return context.Target.Value;
        }
    }
}