// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class CellTree: TreeList<Cell> {
        public CellTree() {}

        public CellTree(Cell value): base(value) {}

        public CellTree(Cell value, params string[] cells): base(value) {
            foreach (var cell in cells) AddBranch(new CellTreeLeaf(cell));
        }

        public CellTree(params string[] cells) {
            foreach (var cell in cells) AddBranch(new CellTreeLeaf(cell));
        }

        public CellTree(params Tree<Cell>[] lists) {
            foreach (var list in lists) AddBranch(list);
        }

        public CellTree(IEnumerable<Tree<Cell>> trees) {
            foreach (var tree in trees) AddBranch(tree);
        }
    }

    public class CellTreeLeaf: CellTree {
        public CellTreeLeaf(string text): base(new CellBase(text)) {}

        public string Text => Value.Text;
    }
}
