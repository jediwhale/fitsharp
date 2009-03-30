// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Engine {
    public class CellOperation {
        private readonly Processor<Cell> processor;

        public CellOperation(Processor<Cell> processor) {
            this.processor = processor;
        }

        public void Create(Fixture fixture, string className, Parse parameterCell) {
            TypedValue instance = processor.Create(className, new CellRange(parameterCell, 1));
            fixture.SetSystemUnderTest(instance.Value);
        }

        public void Input(Fixture fixture, Tree<Cell> memberName, Parse cell) {
            processor.Execute(
                ExecuteContext.Make(fixture), 
                ExecuteParameters.MakeInput(memberName, cell));
        }

        public void Check(Fixture fixture, Tree<Cell> memberName, Tree<Cell> parameters, Parse expectedCell) {
            processor.Execute(
                ExecuteContext.Make(fixture), 
                ExecuteParameters.MakeCheck(memberName, parameters, expectedCell));
        }

        public void Check(Fixture fixture, Tree<Cell> memberName, Parse expectedCell) {
            Check(fixture, memberName, new TreeList<Cell>(), expectedCell);
        }

        public void Check(Fixture fixture, TypedValue actualValue, Parse expectedCell) {
            processor.Execute(
                ExecuteContext.Make(fixture, actualValue),
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
            return Invoke(target, memberName, new CellRange(new Parse[] {}));
        }

        public TypedValue Invoke(object target, Tree<Cell> memberName, Tree<Cell> parameters) {
            TypedValue result = TryInvoke(target, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public bool Compare(TypedValue actual, Parse expectedCell) {
            return (bool)processor.Execute(
                             ExecuteContext.Make(actual), 
                             ExecuteParameters.MakeCompare(expectedCell)).Value;
        }
    }
}