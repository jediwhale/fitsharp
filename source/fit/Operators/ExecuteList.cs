// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Linq;
using fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ExecuteList: InvokeCommandBase, ParseOperator<Cell> {
        public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
            switch (context.Command) {
                case ExecuteCommand.Check:
                    var cell1 = (Parse)parameters.Cell;
                    return cell1.Parts != null && typeof(IList).IsAssignableFrom(GetTypedActual(context, parameters).Type);

                default:
                    return false;
            }
        }

        public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
            ExecuteCheck(context, parameters);
            return TypedValue.Void;
        }

        private void ExecuteCheck(ExecuteContext context, ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(Processor, cell.Parts.Parts));
            matcher.MarkCell(context.SystemUnderTest.Value, GetActual(context, parameters), cell.Parts.Parts);
        }

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof(IList).IsAssignableFrom(type) && parameters.Branches.Count > 0;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var cell = (Parse) parameters;
            //if (cell.Parts == null) throw new FitFailureException("No embedded table.");
            Parse headerCells = cell.Parts.Parts.Parts;
            Parse dataRows = cell.Parts.Parts.More;
            return new TypedValue(
                new CellRange(dataRows).Cells.Aggregate(new ArrayList(), (list, row) => {
                    list.Add(
                        new CellOperationImpl(Processor).Invoke(instance.Value, new CellRange(headerCells),
                                                                new CellRange(row.Parts), row.Parts).Value);
                    return list;
                }));
        }
    }
}
