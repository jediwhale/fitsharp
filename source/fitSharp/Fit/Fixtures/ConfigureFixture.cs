// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class ConfigureFixture: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            processor.TestStatus.TableCount--;

            var facility = processor.Memory.GetItem(table.Branches[0].Branches[1].Value.Text);
            var currentRow = new EnumeratedTree<Cell>(table.Branches[0].Branches.Skip(2));
            var selector = new DoRowSelector();
            var result = new CellOperationImpl(processor).TryInvoke(facility, selector.SelectMethodCells(currentRow),
                                                                    selector.SelectParameterCells(currentRow),
                                                                    currentRow.Branches[0].Value);
            result.ThrowExceptionIfNotValid();
            if (result.IsVoid) return;
            table.Branches[0].Branches[2].Value.SetAttribute(CellAttribute.Folded, result.ValueString);
        }
    }
}
