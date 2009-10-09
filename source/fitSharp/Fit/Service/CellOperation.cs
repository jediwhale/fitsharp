// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public interface CellOperation {
        void Check(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> expectedCell);
        void Check(object systemUnderTest, TypedValue actualValue, Tree<Cell> expectedCell);
        bool Compare(TypedValue actual, Tree<Cell> expectedCell);
        TypedValue Create(string className, Tree<Cell> parameterCell);
        void Input(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> cell);
        TypedValue TryInvoke(object target, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> targetCell);
    }

    public static class CellOperationExtension {
        public static void Check(this CellOperation operation, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> expectedCell) {
            operation.Check(systemUnderTest, memberName, new TreeList<Cell>(), expectedCell);
        }

        public static void Create(this CellOperation operation, MutableDomainAdapter adapter, string className, Tree<Cell> parameterCell) {
            TypedValue instance = operation.Create(className, parameterCell);
            adapter.SetSystemUnderTest(instance.Value);
        }

        public static TypedValue Create(this CellOperation operation, string className) {
            return operation.Create(className, new CellTree());
        }

        public static TypedValue Invoke(this CellOperation operation, object target, Tree<Cell> memberName) {
            return operation.Invoke(target, memberName, new TreeList<Cell>());
        }

        public static TypedValue Invoke(this CellOperation operation, object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            TypedValue result = operation.TryInvoke(target, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue Invoke(this CellOperation operation, object target, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> targetCell) {
            TypedValue result = operation.TryInvoke(target, memberName, parameters, targetCell);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue TryInvoke(this CellOperation operation, object target, Tree<Cell> memberName) {
            return operation.TryInvoke(target, memberName, new TreeList<Cell>());
        }

        public static TypedValue TryInvoke(this CellOperation operation, object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            return operation.TryInvoke(target, memberName, parameters, null);
        }

    }

    public class CellOperationImpl: CellOperation {
        private readonly CellProcessor processor;

        public CellOperationImpl(CellProcessor processor) {
            this.processor = processor;
        }

        public TypedValue Create(string className, Tree<Cell> parameterCell) {
            return processor.Create(className, parameterCell);
        }

        public void Input(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> cell) {
            processor.Execute(
                ExecuteContext.Make(ExecuteCommand.Input, systemUnderTest), 
                ExecuteParameters.MakeMemberCell(memberName, cell));
        }

        public void Check(object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> expectedCell) {
            processor.Execute(
                ExecuteContext.Make(ExecuteCommand.Check, systemUnderTest), 
                ExecuteParameters.Make(memberName, parameters, expectedCell));
        }

        public void Check(object systemUnderTest, TypedValue actualValue, Tree<Cell> expectedCell) {
            processor.Execute(
                ExecuteContext.Make(ExecuteCommand.Check, systemUnderTest, actualValue),
                ExecuteParameters.Make(expectedCell));
        }

        public TypedValue TryInvoke(object target, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> targetCell) {
            return processor.Execute(
                ExecuteContext.Make(ExecuteCommand.Invoke, new TypedValue(target)), 
                ExecuteParameters.Make(memberName, parameters, targetCell));
        }

        public bool Compare(TypedValue actual, Tree<Cell> expectedCell) {
            return (bool)processor.Execute(
                             ExecuteContext.Make(ExecuteCommand.Compare, actual), 
                             ExecuteParameters.Make(expectedCell)).Value;
        }
    }
}