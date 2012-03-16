// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InterpretFlow: InterpretTableFlow {
        public InterpretFlow(): this(0) {}

        public InterpretFlow(int rowsToSkip) {
            this.rowsToSkip = rowsToSkip;
        }

        public void DoTableFlow(CellProcessor processor, FlowInterpreter interpreter, Tree<Cell> table) {
            this.interpreter = interpreter;
            this.processor = processor;

            if (processor.TestStatus.IsAbandoned) return;
            hasFinishedTable = false;
            for (var i = 0; i < table.Branches.Count; i++) {
                if (i < rowsToSkip) continue;
                if (hasFinishedTable) break;
                ProcessFlowRow(table, i);
            }
	    }

        void ProcessFlowRow(Tree<Cell> table, int rowNumber) {
            var currentRow = table.Branches[rowNumber];
            try
            {
                var specialActionName = processor.ParseTree<Cell, MemberName>(currentRow.Branches[0]).AsSpecialAction();
                var result = processor.Invoke(interpreter, specialActionName, currentRow.Branches[0]);
                if (!result.IsValid) {
                     result = processor.Execute(interpreter,
                         interpreter.MethodRowSelector.SelectMethodCells(currentRow),
                         interpreter.MethodRowSelector.SelectParameterCells(currentRow),
                         currentRow.ValueAt(0));

                }
                if (!result.IsValid) {
                    if (result.IsException<MemberMissingException>() && currentRow.ValueAt(0).Text.Length > 0) {
                        var newFixture = processor.ParseTree<Cell, Interpreter>(currentRow);
                        // if its first row and new one isinflow 
                        //   callback to see if switch?  or just callback on new fixture created?
                        // if callback is no switch
                        var adapter = newFixture as MutableDomainAdapter;
                        if (adapter != null) adapter.SetSystemUnderTest(interpreter.SystemUnderTest);
                        // endif
                        ProcessRestOfTable(newFixture, MakeTableWithRows(table, rowNumber));
                    }
                    else {
                        result.ThrowExceptionIfNotValid();
                    }
                }
                else {

                    if (processor.TestStatus.IsAbandoned) {
                        processor.TestStatus.MarkIgnore(currentRow.Value);
                        return;
                    }
                    if (result.Type == typeof(bool)) {
                        ColorMethodName(interpreter.MethodRowSelector.SelectMethodCells(currentRow), result.GetValue<bool>());
                    }
                    else {
                        //todo: change wrapping re sut & call stack?
                        processor.Operate<WrapOperator>(result)
                            .As<Interpreter>(i => ProcessRestOfTable(i, MakeTableWithRows(table, rowNumber)));
                    }
                }
            }
            catch (IgnoredException) {}
	        catch (ParseException<Cell> e) {
	            processor.TestStatus.MarkException(e.Subject, e);
                hasFinishedTable = true;
	        }
            catch (System.Exception e) {
                processor.TestStatus.MarkException(currentRow.ValueAt(0), e);
                hasFinishedTable = true;
            }
        }

        Tree<Cell> MakeTableWithRows(Tree<Cell> table, int rowNumber) {
            var rows = new List<Tree<Cell>>(table.Branches.Skip(rowNumber));
            return processor.MakeCell(string.Empty, "table", rows);
        }

        void ProcessRestOfTable(Interpreter childInterpreter, Tree<Cell> theRestOfTheRows) {
            processor.CallStack.Push();
            processor.CallStack.DomainAdapter = new TypedValue(interpreter);
            try {
                RunTestDefault.DoTable(theRestOfTheRows, childInterpreter, processor, false);
            }
            catch (System.Exception e) {
                processor.TestStatus.MarkException(theRestOfTheRows.ValueAt(0, 0), e);
            }
            processor.CallStack.PopReturn();
            hasFinishedTable = true;
        }

        void ColorMethodName(Tree<Cell> methodCells, bool isRight) {
            foreach (var nameCell in methodCells.Branches) {
                processor.TestStatus.ColorCell(nameCell.Value, isRight);
            }
        }

        readonly int rowsToSkip;

        CellProcessor processor;
        FlowInterpreter interpreter;
        bool hasFinishedTable;
    }
}
