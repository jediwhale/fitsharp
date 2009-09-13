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
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ExecuteList: ExecuteBase, ParseOperator<Cell> {
        public override bool CanExecute(ExecuteParameters parameters) {
            switch (parameters.Verb) {
                case ExecuteParameters.Check:
                    var cell1 = (Parse)parameters.Cell;
                    return cell1.Parts != null && typeof(IList).IsAssignableFrom(parameters.GetTypedActual(this).Type);

                case ExecuteParameters.Compare:
                    var cell2 = (Parse)parameters.Cell;
                    return cell2.Parts != null && typeof(IList).IsAssignableFrom(parameters.Target.Type);

                default:
                    return false;
            }
        }

        public override TypedValue Execute(ExecuteParameters parameters) {
            switch (parameters.Verb) {
                case ExecuteParameters.Check:
                    ExecuteCheck(parameters);
                    break;

                case ExecuteParameters.Compare:
                    return ExecuteEvaluate(parameters);

            }
            return TypedValue.Void;
        }

        private TypedValue ExecuteEvaluate(ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(cell.Parts.Parts));
            return new TypedValue(matcher.IsEqual(parameters.Target.Value, cell));
        }

        private void ExecuteCheck(ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(cell.Parts.Parts));
            matcher.MarkCell(parameters.TestStatus, parameters.SystemUnderTest.Value, GetActual(parameters), cell.Parts.Parts);
        }

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof(IList).IsAssignableFrom(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var cell = (Parse) parameters;
            if (cell.Parts == null) throw new FitFailureException("No embedded table.");
            Parse headerCells = cell.Parts.Parts.Parts;
            var list = new ArrayList();
            foreach (Parse row in new CellRange(cell.Parts.Parts.More).Cells) {
                list.Add(
                    new CellOperation(Processor).Invoke(instance.Value, new CellRange(headerCells), new CellRange(row.Parts)).Value);
            }
            return new TypedValue(list);
        }
    }
}
