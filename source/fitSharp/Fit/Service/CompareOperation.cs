// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class CompareOperation {
        public TypedValue Target { get; private set; }
        public Tree<Cell> Cells { get; private set; }

        public CompareOperation(CellProcessor processor, TypedValue target, Tree<Cell> cells) {
            Target = target;
            Cells = cells;
            this.processor = processor;
        }

        public bool Do() {
            return processor.Compare(Target, Cells);
        }

        readonly CellProcessor processor;
    }
}
