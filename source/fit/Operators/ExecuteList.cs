// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Extension;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ExecuteList: ExecuteBase, ParseOperator<Cell> {
        public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
            switch (context.Command) {
                case ExecuteCommand.Check:
                    var cell1 = (Parse)parameters.Cell;
                    return cell1.Parts != null && typeof(IList).IsAssignableFrom(GetTypedActual(context, parameters).Type);
                case ExecuteCommand.Compare:
                    var cell2 = (Parse)parameters.Cell;
                    return cell2.Parts != null && typeof(IList).IsAssignableFrom(context.Target.Value.Type);

                default:
                    return false;
            }
        }

        public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
            switch (context.Command) {
                case ExecuteCommand.Check:
                    ExecuteCheck(context, parameters);
                    break;
                case ExecuteCommand.Compare:
                    return ExecuteEvaluate(context, parameters);
            }
            return TypedValue.Void;
        }

        private TypedValue ExecuteEvaluate(ExecuteContext context, ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(cell.Parts.Parts));
            return new TypedValue(matcher.IsEqual(context.Target.Value.Value, cell));
        }

        private void ExecuteCheck(ExecuteContext context, ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(cell.Parts.Parts));
            matcher.MarkCell(context.SystemUnderTest.Value, GetActual(context, parameters), cell.Parts.Parts);
        }

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof(IList).IsAssignableFrom(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var cell = (Parse) parameters;
            if (cell.Parts == null) throw new FitFailureException("No embedded table.");
            Parse headerCells = cell.Parts.Parts.Parts;
            Parse dataRows = cell.Parts.Parts.More;
            return new TypedValue(
                new CellRange(dataRows).Cells.Aggregate((ArrayList list, Parse row) =>
                    list.Add(new CellOperationImpl(Processor).Invoke(instance.Value, new CellRange(headerCells), new CellRange(row.Parts), row.Parts).Value)));
        }
    }
}
