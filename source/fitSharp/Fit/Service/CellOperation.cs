// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public interface CellOperation {
        void Check(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> expectedCell);
        void Check(TypedValue actualValue, Tree<Cell> expectedCell);
        TypedValue TryInvoke(object target, Tree<Cell> memberName, Tree<Cell> parameters, Cell targetCell);
    }

    public static class CellOperationExtension {
        public static void Check(this CellOperation operation, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> expectedCell) {
            operation.Check(systemUnderTest, memberName, new CellTree(), expectedCell);
        }

        public static TypedValue Invoke(this CellOperation operation, object target, Tree<Cell> memberName) {
            return operation.Invoke(target, memberName, new CellTree());
        }

        public static TypedValue Invoke(this CellOperation operation, object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            TypedValue result = operation.TryInvoke(target, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue Invoke(this CellOperation operation, object target, Tree<Cell> memberName, Tree<Cell> parameters, Cell targetCell) {
            TypedValue result = operation.TryInvoke(target, memberName, parameters, targetCell);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue TryInvoke(this CellOperation operation, object target, Tree<Cell> memberName) {
            return operation.TryInvoke(target, memberName, new CellTree());
        }

        public static TypedValue TryInvoke(this CellOperation operation, object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            return operation.TryInvoke(target, memberName, parameters, new CellBase(string.Empty));
        }

    }

    public class CellOperationImpl: CellOperation {
        public CellOperationImpl(CellProcessor processor) {
            this.processor = processor;
        }

        public void Check(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> expectedCell) {
            processor.Operate<CheckOperator>(
                CellOperationValue.Make(systemUnderTest, memberName, parameters),
                expectedCell);
        }

        public void Check(TypedValue actualValue, Tree<Cell> expectedCell) {
            processor.Operate<CheckOperator>(
                CellOperationValue.Make(actualValue),
                expectedCell);
        }

        public TypedValue TryInvoke(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Cell targetCell) {
            var beforeCounts = new TestCounts(processor.TestStatus.Counts);
            var targetObjectProvider = systemUnderTest as TargetObjectProvider;
            var name = processor.ParseTree<Cell, MemberName>(memberName);
            var result = processor.Invoke(
                    new TypedValue(targetObjectProvider != null ? targetObjectProvider.GetTargetObject() : systemUnderTest),
                    name, parameters);
            processor.TestStatus.MarkCellWithLastResults(new CellTree(targetCell), beforeCounts);
            return result;
        }

        readonly CellProcessor processor;
    }
}
