// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class CellOperation {
        private readonly Processor<Cell> processor;

        public CellOperation(Processor<Cell> processor) {
            this.processor = processor;
        }

        public void Create(MutableDomainAdapter adapter, string className, Tree<Cell> parameterCell) {
            TypedValue instance = processor.Create(className, parameterCell);
            adapter.SetSystemUnderTest(instance.Value);
        }

        public void Input(TestStatus testStatus, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> cell) {
            processor.Execute(
                ExecuteContext.Make(testStatus, systemUnderTest), 
                ExecuteParameters.MakeInput(memberName, cell));
        }

        public void Check(TestStatus testStatus, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> parameters, Tree<Cell> expectedCell) {
            processor.Execute(
                ExecuteContext.Make(testStatus, systemUnderTest), 
                ExecuteParameters.MakeCheck(memberName, parameters, expectedCell));
        }

        public void Check(TestStatus testStatus, object systemUnderTest, Tree<Cell> memberName, Tree<Cell> expectedCell) {
            Check(testStatus, systemUnderTest, memberName, new TreeList<Cell>(), expectedCell);
        }

        public void Check(TestStatus testStatus, object systemUnderTest, TypedValue actualValue, Tree<Cell> expectedCell) {
            processor.Execute(
                ExecuteContext.Make(testStatus, systemUnderTest, actualValue),
                ExecuteParameters.MakeCheck(expectedCell));
        }

        public TypedValue TryInvoke(object target, Tree<Cell> memberName) {
            return TryInvoke(target, memberName, new TreeList<Cell>());
        }

        public TypedValue TryInvoke(object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            return processor.Execute(
                ExecuteContext.Make(new TypedValue(target)), 
                ExecuteParameters.MakeInvoke(memberName, parameters));
        }

        public TypedValue Invoke(object target, Tree<Cell> memberName) {
            return Invoke(target, memberName, new TreeList<Cell>());
        }

        public TypedValue Invoke(object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            TypedValue result = TryInvoke(target, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public bool Compare(TypedValue actual, Tree<Cell> expectedCell) {
            return (bool)processor.Execute(
                             ExecuteContext.Make(actual), 
                             ExecuteParameters.MakeCompare(expectedCell)).Value;
        }
    }
}