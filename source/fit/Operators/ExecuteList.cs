// FitNesse.NET
// Copyright © 2007,2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fit.Engine;
using fit.Model;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ExecuteList: ExecuteBase, ParseOperator<Cell> {

        public override bool TryExecute(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result) {
            switch (parameters.Verb) {
                case ExecuteParameters.Check:
                    return ExecuteCheck(processor, parameters);

                case ExecuteParameters.Compare:
                    return ExecuteEvaluate(processor, parameters, ref result);

                default:
                    return false;
            }
        }

        private static bool ExecuteEvaluate(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result) {
            var cell = (Parse)parameters.Cell;
            if (!typeof(IList).IsAssignableFrom(parameters.Target.Type)) return false;
            if (cell.Parts == null) return false;

            var matcher = new ListMatcher(processor, new ArrayMatchStrategy(cell.Parts.Parts));
            result = new TypedValue(matcher.IsEqual(parameters.Target.Value, cell));
            return true;
        }

        private static bool ExecuteCheck(Processor<Cell> processor, ExecuteParameters parameters) {
            var cell = (Parse)parameters.Cell;
            if (cell.Parts == null) return false;
            if (!typeof(IList).IsAssignableFrom(parameters.GetTypedActual(processor).Type)) return false;

            var matcher = new ListMatcher(processor, new ArrayMatchStrategy(cell.Parts.Parts));
            matcher.MarkCell(processor, parameters.TestStatus, parameters.SystemUnderTest.Value, parameters.GetActual(processor), cell.Parts.Parts,  cell.Parts.Parts.More);
            return true;
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
