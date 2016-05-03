// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Fixtures;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeFlowKeyword: CellOperator, InvokeSpecialOperator {

        public InvokeFlowKeyword() {
            keywords.Add("check", Check);
            keywords.Add("checkfieldsfor", CheckFieldsFor);
            keywords.Add("comment", Comment);
            keywords.Add("ensure", Ensure);
            keywords.Add("name", Name);
            keywords.Add("not", Not);
            keywords.Add("note", Note);
            keywords.Add("reject", Not);
            keywords.Add("return", Return);
            keywords.Add("set", Set);
            keywords.Add("show", Show);
            keywords.Add("showas", ShowAs);
            keywords.Add("start", Start);
            keywords.Add("waituntil", WaitUntil);
            keywords.Add("with", With);
        }

        public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return keywords.ContainsKey(memberName.Name);
        }

        public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return keywords[memberName.Name](instance.GetValue<FlowInterpreter>(), parameters);
        }

        TypedValue Check(FlowInterpreter interpreter, Tree<Cell> row) {
            DoCheckOperation(interpreter, row, false);
            return TypedValue.Void;
        }

        TypedValue CheckFieldsFor(FlowInterpreter interpreter, Tree<Cell> row) {
            return new TypedValue(new List<object> { interpreter.ExecuteFlowRowMethod(Processor, row) });
        }

        TypedValue Comment(FlowInterpreter interpreter, Tree<Cell> row) {
            return new TypedValue(new CommentFixture());
        }

        TypedValue Ensure(FlowInterpreter interpreter, Tree<Cell> row) {
            var firstCell = row.Branches[0].Value;
            try {
                Processor.TestStatus.ColorCell(firstCell, (bool) interpreter.ExecuteFlowRowMethod(Processor, row));
            }
            catch (IgnoredException) {}
            catch (System.Exception e) {
                Processor.TestStatus.MarkException(firstCell, e);
            }
            return TypedValue.Void;
        }

        TypedValue Name(FlowInterpreter interpreter, Tree<Cell> row) {
            if (row.Branches.Count < 3) {
                throw new TableStructureException("missing cells for name.");
            }

            var namedValue = withIdentifier.Equals(row.Branches[2].Value.Text)
                                    ? new MethodPhrase(row.Skip(2)).Evaluate(interpreter, Processor)
                                    : interpreter.ExecuteFlowRowMethod(Processor, row.Skip(1));
            Processor.Get<Symbols>().Save(row.Branches[1].Value.Text, namedValue);
            Processor.TestStatus.MarkRight(row.Branches[1].Value);
            return TypedValue.Void;
        }

        TypedValue Not(FlowInterpreter interpreter, Tree<Cell> row) {
            var firstCell = row.Branches[0].Value;
            try {
                Processor.TestStatus.ColorCell(firstCell, !(bool) interpreter.ExecuteFlowRowMethod(Processor, row));
            }
            catch (IgnoredException) {}
            catch (System.Exception) {
                Processor.TestStatus.MarkRight(firstCell);
            }
            return TypedValue.Void;
        }

        TypedValue Note(FlowInterpreter interpreter, Tree<Cell> row) { return TypedValue.Void; }

        TypedValue Return(FlowInterpreter interpreter, Tree<Cell> row) {
            var result = new MethodPhrase(row).Evaluate(interpreter, Processor);
            Processor.CallStack.SetReturn(new TypedValue(result));
            return TypedValue.Void;
        }

        TypedValue Set(FlowInterpreter interpreter, Tree<Cell> row) {
            interpreter.ExecuteFlowRowMethod(Processor, row);
            return TypedValue.Void;
        }

        TypedValue Show(FlowInterpreter interpreter, Tree<Cell> row) {
            try {
                AddCell(row, interpreter.ExecuteFlowRowMethod(Processor, row));
            }
            catch (IgnoredException) {}
            return TypedValue.Void;
        }

        TypedValue ShowAs(FlowInterpreter interpreter, Tree<Cell> row) {
            try {
                var restOfRow = row.Skip(1);
                var attributes = GetAttributes(Processor.Parse<Cell, string>(row.Branches[1].Value));
                var value = interpreter.ExecuteFlowRowMethod(Processor, restOfRow);
                AddCell(row, new ComposeShowAsOperator(attributes, value));
            }
            catch (IgnoredException) {}
            return TypedValue.Void;
        }


        TypedValue Start(FlowInterpreter interpreter, Tree<Cell> row) {
            try {
                interpreter.SetSystemUnderTest(new MethodPhrase(row).EvaluateNew(Processor));
            }
            catch (System.Exception e) {
                Processor.TestStatus.MarkException(row.Branches[0].Value, e);
            }
            return TypedValue.Void;
        }

        TypedValue WaitUntil(FlowInterpreter interpreter, Tree<Cell> row) {
            DoCheckOperation(interpreter, row, true);
            return TypedValue.Void;
        }

        TypedValue With(FlowInterpreter interpreter, Tree<Cell> row) {
            interpreter.SetSystemUnderTest(new MethodPhrase(row).Evaluate(interpreter, Processor));
            return TypedValue.Void;
        }

        void AddCell(Tree<Cell> row, object theNewValue) {
            row.Add(Processor.Compose(theNewValue));
        }

        void DoCheckOperation(FlowInterpreter interpreter, Tree<Cell> row, bool isVolatile) {
            try {
                var methodCells = GetMethodCellRange(row, 1);
                try {
                    Processor.Operate<CheckOperator>(
                        CellOperationValue.Make(
                            interpreter,
                            interpreter.MethodRowSelector.SelectMethodCells(methodCells),
                            interpreter.MethodRowSelector.SelectParameterCells(methodCells),
                            isVolatile),
                        row.Last());

                }
                catch (MemberMissingException e) {
                    Processor.TestStatus.MarkException(row.Branches[1].Value, e);
                }
                catch (System.Exception e) {
                    Processor.TestStatus.MarkException(row.Last().Value, e);
                }
            }
            catch (IgnoredException) {}
        }

        static Tree<Cell> GetMethodCellRange(Tree<Cell> row , int excludedCellCount) {
            if (row.Branches.Count < 2) {
                throw new FitFailureException("Missing cells for embedded method");
            }
            return row.Skip(1).Take(row.Branches.Count -excludedCellCount - 1);
        }

        static IEnumerable<CellAttribute> GetAttributes(string list) {
            return list.Split(',').Select(item => showAsAttributes[item.Trim().ToLower()]);
        }

        static readonly IdentifierName withIdentifier = new IdentifierName("with");

        static readonly Dictionary<string, CellAttribute> showAsAttributes = new Dictionary<string, CellAttribute> {
            {"folded", CellAttribute.Folded},
            {"formatted", CellAttribute.Formatted},
            {"raw", CellAttribute.Raw}
        };

        readonly Dictionary<string, Func<FlowInterpreter, Tree<Cell>, TypedValue>> keywords = new Dictionary<string, Func<FlowInterpreter, Tree<Cell>, TypedValue>>();
    }
}
