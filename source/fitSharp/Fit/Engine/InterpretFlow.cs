// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public class InterpretFlow: InterpretTableFlow {
        public InterpretFlow(CellProcessor processor, FlowInterpreter interpreter) {
            this.interpreter = interpreter;
            this.processor = processor;
        }

        public InterpretFlow OnNewFixture(Func<Interpreter, int, bool> onNewFixture) {
            this.onNewFixture = onNewFixture;
            return this;
        }

        public void DoTableFlow(Tree<Cell> table, int rowsToSkip) {
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
            try {
                var result = currentRow.InvokeSpecialAction(processor, interpreter);

                if (!result.IsValid) {
                    result = currentRow.ExecuteMethod(processor, interpreter);
                }

                if (!result.IsValid) {
                    if (result.IsException<MemberMissingException>()) {
                        foreach (var cell in interpreter.MethodRowSelector.SelectMethodCells(currentRow).Branches) {
                            cell.Value.ClearAttribute(CellAttribute.Syntax);
                        }
                        var newFixture = processor.ParseTree<Cell, Interpreter>(currentRow);
                        var newFlow = onNewFixture(newFixture, rowNumber);
                        var adapter = newFixture as MutableDomainAdapter;
                        if (adapter != null && adapter.SystemUnderTest == null && interpreter.SystemUnderTest != null)
                            adapter.SetSystemUnderTest(interpreter.SystemUnderTest);
                        ProcessRestOfTable(newFixture, MakeTableWithRows(table, rowNumber), newFlow);
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
                            .As<Interpreter>(i => ProcessRestOfTable(i, MakeTableWithRows(table, rowNumber), false));
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
            if (rowNumber == 0) return table;
            var rows = new List<Tree<Cell>>(table.Branches.Skip(rowNumber));
            return processor.MakeCell(string.Empty, "table", rows);
        }

        void ProcessRestOfTable(Interpreter childInterpreter, Tree<Cell> theRestOfTheRows, bool isFlow) {
            processor.CallStack.Push();
            processor.CallStack.DomainAdapter = new TypedValue(interpreter);
            try {
                DoTable(theRestOfTheRows, childInterpreter, isFlow);
            }
            catch (System.Exception e) {
                processor.TestStatus.MarkException(theRestOfTheRows.ValueAt(0, 0), e);
            }
            processor.CallStack.PopReturn();
            hasFinishedTable = true;
        }

        //todo: flag argument - yuck
        void DoTable(Tree<Cell> table, Interpreter activeFixture, bool inFlow) {
            var activeFlowFixture = activeFixture as FlowInterpreter;
            if (activeFlowFixture != null) activeFlowFixture.DoSetUp(processor, table);
            activeFixture.Interpret(processor, table);
            if (activeFlowFixture != null && !inFlow) activeFlowFixture.DoTearDown(table);
        }

        void ColorMethodName(Tree<Cell> methodCells, bool isRight) {
            foreach (var nameCell in methodCells.Branches) {
                processor.TestStatus.ColorCell(nameCell.Value, isRight);
            }
        }

        readonly CellProcessor processor;
        readonly FlowInterpreter interpreter;
        Func<Interpreter, int, bool> onNewFixture = (i, r) => false;
        bool hasFinishedTable;
    }
}
