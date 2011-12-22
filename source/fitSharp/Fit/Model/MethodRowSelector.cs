// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public interface MethodRowSelector {
        Tree<Cell> SelectMethodCells(Tree<Cell> row);
        Tree<Cell> SelectParameterCells(Tree<Cell> row);
    }

    public class DoRowSelector: MethodRowSelector {
        public Tree<Cell> SelectMethodCells(Tree<Cell> row) {
            return new CellTree(row.Branches.Alternate());
        }

        public Tree<Cell> SelectParameterCells(Tree<Cell> row) {
            return new CellTree(row.Branches.Skip(1).Alternate());
        }
    }

    public class SequenceRowSelector: MethodRowSelector {
        public Tree<Cell> SelectMethodCells(Tree<Cell> row) {
            return new CellTree(row.Branches.Take(1));
        }

        public Tree<Cell> SelectParameterCells(Tree<Cell> row) {
            return new CellTree(row.Branches.Skip(1));
        }
    }
}
