// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public abstract class CellOperator: Operator<Cell, CellProcessor> { //todo: rename? move?
        public void MarkCellWithLastResults(Tree<Cell> parameters, Action<Cell> markWithCounts) {
            var cell = parameters == null ? null : parameters.Value;
            if (cell != null && !string.IsNullOrEmpty(Processor.TestStatus.LastAction)) {
                cell.SetAttribute(CellAttribute.Folded, Processor.TestStatus.LastAction);
                markWithCounts(cell);
            }
            Processor.TestStatus.LastAction = null;
        }
    }
}