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
        public override bool IsMatch(Processor<Cell> processor, ExecuteParameters parameters) {
            switch (parameters.Verb) {
                case ExecuteParameters.Check:
                    var cell1 = (Parse)parameters.Cell;
                    return cell1.Parts != null && typeof(IList).IsAssignableFrom(parameters.GetTypedActual(processor).Type);

                case ExecuteParameters.Compare:
                    var cell2 = (Parse)parameters.Cell;
                    return cell2.Parts != null && typeof(IList).IsAssignableFrom(parameters.Target.Type);

                default:
                    return false;
            }
        }

        public override TypedValue Execute(Processor<Cell> processor, ExecuteParameters parameters) {
            switch (parameters.Verb) {
                case ExecuteParameters.Check:
                    ExecuteCheck(processor, parameters);
                    break;

                case ExecuteParameters.Compare:
                    return ExecuteEvaluate(processor, parameters);

            }
            return TypedValue.Void;
        }

        private static TypedValue ExecuteEvaluate(Processor<Cell> processor, ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(processor, new ArrayMatchStrategy(cell.Parts.Parts));
            return new TypedValue(matcher.IsEqual(parameters.Target.Value, cell));
        }

        private static void ExecuteCheck(Processor<Cell> processor, ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            var matcher = new ListMatcher(processor, new ArrayMatchStrategy(cell.Parts.Parts));
            matcher.MarkCell(parameters.TestStatus, parameters.SystemUnderTest.Value, parameters.GetActual(processor), cell.Parts.Parts);
        }

        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (!typeof(IList).IsAssignableFrom(type)) return false;

            var cell = (Parse) parameters;
            if (cell.Parts == null) throw new FitFailureException("No embedded table.");
            Parse headerCells = cell.Parts.Parts.Parts;
            var list = new ArrayList();
            foreach (Parse row in new CellRange(cell.Parts.Parts.More).Cells) {
                list.Add(
                    new CellOperation(processor).Invoke(instance.Value, new CellRange(headerCells), new CellRange(row.Parts)).Value);
            }
            result = new TypedValue(list);
            return true;
        }
    }
}
