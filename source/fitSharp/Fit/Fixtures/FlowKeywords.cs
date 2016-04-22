// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class FlowKeywords {

        public FlowKeywords(FlowInterpreter fixture, CellProcessor processor) {
            this.fixture = fixture;
            this.processor = processor;
        }

        public void Ensure(Tree<Cell> row) {
            var firstCell = row.Branches[0].Value;
            try {
                processor.TestStatus.ColorCell(firstCell, (bool) ExecuteEmbeddedMethod(row));
            }
            catch (IgnoredException) {}
            catch (System.Exception e) {
                processor.TestStatus.MarkException(firstCell, e);
            }
        }

        public void Set(Tree<Cell> row) {
            ExecuteEmbeddedMethod(row);
        }

        object ExecuteEmbeddedMethod(Tree<Cell> row) {
            return fixture.ExecuteFlowRowMethod(processor, row);
        }

        readonly FlowInterpreter fixture;
        readonly CellProcessor processor;
    }
}
