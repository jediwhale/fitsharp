// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service
{
    public class InterpretFlow: InterpretTableFlow {
        public InterpretFlow(): this(0) {}

        public InterpretFlow(int rowsToSkip) {
            this.rowsToSkip = rowsToSkip;
        }

        public void DoTableFlow(CellProcessor processor, FlowInterpreter interpreter, Tree<Cell> table) {
            this.interpreter = interpreter;
            this.processor = processor;

            if (interpreter.TestStatus.IsAbandoned) return;
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
                var specialActionName = InvokeDirect.MakeDirect(
                    processor.ParseTree<Cell, MemberName>(currentRow.Branches[0]).ToString());
                var runtimeType = processor.ParseString<Cell, RuntimeType>("fit.Fixtures.FlowKeywords");
                var runtimeMember = runtimeType.GetConstructor(1);
                var flowKeywords = runtimeMember.Invoke(new object[] {interpreter});
                var result = processor.Invoke(flowKeywords, specialActionName, currentRow.Branches[0]);
                if (!result.IsValid) {
                    result = processor.Invoke(interpreter, specialActionName, currentRow.Branches[0]);
                }
                if (!result.IsValid) {
                     result = new CellOperationImpl(processor).TryInvoke(interpreter,
                         interpreter.MethodRowSelector.SelectMethodCells(currentRow),
                         interpreter.MethodRowSelector.SelectParameterCells(currentRow),
                        currentRow.Branches[0].Value);

                }
                if (!result.IsValid) {
                    if (result.IsException<MemberMissingException>() && currentRow.Branches[0].Value.Text.Length > 0) {
                        var newFixture = processor.ParseTree<Cell, Interpreter>(currentRow);
                        var adapter = newFixture as MutableDomainAdapter;
                        if (adapter != null) adapter.SetSystemUnderTest(interpreter.SystemUnderTest);
                        ProcessRestOfTable(newFixture, processor.MakeCell(string.Empty, table.Branches.Skip(rowNumber)));
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
                        new InterpretResult(processor).Interpret(result,
                            i => ProcessRestOfTable(i, processor.MakeCell(string.Empty, table.Branches.Skip(rowNumber))));
                    }
                }
            }
            catch (IgnoredException) {}
	        catch (ParseException<Cell> e) {
	            processor.TestStatus.MarkException(e.Subject, e);
                hasFinishedTable = true;
	        }
            catch (System.Exception e) {
                processor.TestStatus.MarkException(currentRow.Branches[0].Value, e);
                hasFinishedTable = true;
            }
        }

        void ProcessRestOfTable(Interpreter childInterpreter, Tree<Cell> theRestOfTheRows) {
            childInterpreter.Prepare(processor, interpreter, theRestOfTheRows.Branches[0]);
            try {
                ExecuteStoryTest.DoTable(theRestOfTheRows, childInterpreter, false);
            }
            catch (System.Exception e) {
                processor.TestStatus.MarkException(theRestOfTheRows.Branches[0].Branches[0].Value, e);
            }
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

    public class InterpretResult {
        public InterpretResult(CellProcessor processor) {
            this.processor = processor;
        }

        public void Interpret(TypedValue result, Action<Interpreter> action) {
            if (result.Value == null) return;
            if (result.Type.IsPrimitive) return;
            if (result.Type == typeof(string)) return;
            var wrapInterpreter = result.GetValueAs<Interpreter>();
            if (wrapInterpreter == null) {
                if (typeof (IEnumerable<object>).IsAssignableFrom(result.Type))
                    wrapInterpreter = MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerable<object>), result.Value);
                else if (typeof (IDictionary).IsAssignableFrom(result.Type))
                    wrapInterpreter = MakeInterpreter("fitlibrary.SetFixture", typeof(IEnumerable), result.GetValue<IDictionary>().Values);
                else if (typeof (IEnumerator).IsAssignableFrom(result.Type))
                    wrapInterpreter = MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerator), result.Value);
                else if (typeof (DataTable).IsAssignableFrom(result.Type))
                    wrapInterpreter = MakeInterpreter("fitlibrary.ArrayFixture", typeof(DataTable), result.Value);
                else if (typeof (XmlDocument).IsAssignableFrom(result.Type))
                    wrapInterpreter = MakeInterpreter("fitlibrary.XmlFixture", typeof(XmlDocument), result.Value);
                else if (typeof (IEnumerable).IsAssignableFrom(result.Type))
                    wrapInterpreter = MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerable), result.Value);
                else wrapInterpreter = MakeInterpreter("fitlibrary.DoFixture", typeof (object), result.Value);
            }
            action(wrapInterpreter);
        }

        Interpreter MakeInterpreter(string fixtureName, Type parameterType, object parameter) {
            var runtimeType = processor.ParseString<Cell, RuntimeType>(fixtureName);
            var runtimeMember = runtimeType.FindConstructor(new [] {parameterType});
            return runtimeMember.Invoke(new [] {parameter}).GetValue<Interpreter>();
        }

        readonly CellProcessor processor;
    }
}
