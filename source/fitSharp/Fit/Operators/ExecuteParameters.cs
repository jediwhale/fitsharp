// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteParameters {

        private readonly Tree<Cell> tree;

        public Cell Cell { get { return tree.Branches[0].Value; } }
        public Tree<Cell> Cells { get { return tree.Branches[0]; } }
        public Tree<Cell> Members { get { return tree.Branches[1]; } }
        public Cell Member { get { return tree.Branches[1].Value; } }
        public Tree<Cell> Parameters { get { return tree.Branches[2]; } }

        public static Tree<Cell> Make(Tree<Cell> cell) {
            return new TreeList<Cell>()
                .AddBranch(cell);
        }

        public static Tree<Cell> Make(Tree<Cell> member, Tree<Cell> parameters, Tree<Cell> expectedCell) {
            return new TreeList<Cell>()
                .AddBranch(expectedCell)
                .AddBranch(member)
                .AddBranch(parameters);
        }

        public static Tree<Cell> MakeMemberCell(Tree<Cell> member, Tree<Cell> inputCell) {
            return new TreeList<Cell>()
                .AddBranch(inputCell)
                .AddBranch(member);
        }

        public ExecuteParameters(Tree<Cell> tree) {
            this.tree = tree;
        }
    }
}