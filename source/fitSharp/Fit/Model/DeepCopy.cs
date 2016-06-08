// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class DeepCopy {
        public DeepCopy(CellProcessor processor) {
            this.processor = processor;
        }

        public Tree<Cell> Make(Tree<Cell> original) {
            return Make(original, source => null);
        }

        public Tree<Cell> Make(Tree<Cell> original, Func<Tree<Cell>, Tree<Cell>> substitute) {
            return Make(original, source => source.Branches, substitute);
        }

        public Tree<Cell> Make(Tree<Cell> original, Func<Tree<Cell>, IEnumerable<Tree<Cell>>> branches, Func<Tree<Cell>, Tree<Cell>> substitute) {
            var source = substitute(original);
            if (source != null) return source;
            var newCell = processor.MakeCell(
                    original.Value == null ? string.Empty : original.Value.Text,
                    string.Empty,
                    branches(original).Select(branch => Make(branch, substitute)));
            if (original.Value != null) {
                foreach (var pair in original.Value.Attributes) {
                    newCell.Value.SetAttribute(pair.Key, pair.Value.Value);
                }
            }
            return newCell;
        }

        readonly CellProcessor processor;
    }
}
