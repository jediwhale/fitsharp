// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model
{
    public interface CellProcessor: Processor<Cell> {
	    TestStatus TestStatus { get; set; }
        CallStack CallStack { get; }
        Tree<Cell> MakeCell(string text, IEnumerable<Tree<Cell>> branches);
    }

    public static class CellProcessorExtension {
        public static void Check(this CellProcessor processor, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> expectedCell) {
            processor.Operate<CheckOperator>(
                CellOperationValue.Make(systemUnderTest, memberName, parameters),
                expectedCell);
        }

        public static void Check(this CellProcessor processor, TypedValue actualValue, Tree<Cell> expectedCell) {
            processor.Operate<CheckOperator>(
                CellOperationValue.Make(actualValue),
                expectedCell);
        }

        public static void Check(this CellProcessor processor, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> expectedCell) {
            processor.Check(systemUnderTest, memberName, new CellTree(), expectedCell);
        }

        public static TypedValue Execute(this CellProcessor processor, object systemUnderTest, Tree<Cell> memberName,
                Tree<Cell> parameters, Cell targetCell) {
            return processor.Operate<ExecuteOperator>(systemUnderTest, memberName, parameters, targetCell);
        }

        public static TypedValue Execute(this CellProcessor processor, object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            return processor.Execute(target, memberName, parameters, new CellBase(string.Empty));
        }

        public static TypedValue ExecuteWithThrow(this CellProcessor processor, object target, Tree<Cell> memberName, Tree<Cell> parameters, Cell targetCell) {
            var result = processor.Execute(target, memberName, parameters, targetCell);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue ExecuteWithThrow(this CellProcessor processor, object target, Tree<Cell> memberName) {
            return processor.ExecuteWithThrow(target, memberName, new CellTree(), new CellBase(string.Empty));
        }

        public static V Get<V>(this CellProcessor processor) where V: new() { return processor.Memory.GetItem<V>(); }
    }
}
