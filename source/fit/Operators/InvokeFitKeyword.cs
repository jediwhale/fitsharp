// Copyright © 2021 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fit.exception;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class InvokeFitKeyword: CellOperator, InvokeSpecialOperator {
        public InvokeFitKeyword() {
            keywords.Add("abandonstorytest", AbandonStoryTest);
            keywords.Add("calculate", Calculate);
            keywords.Add("ignored", Ignored);
        }

        public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return keywords.ContainsKey(memberName.Name) || FindMember(instance.Value, memberName, typeof(Parse)).IsPresent;
        }

        public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            var cell = parameters.Branches[0];

            // lookup Fixture
            foreach (var member in FindMember(instance.Value, memberName, typeof(Parse)).Value) {
                cell.Value.SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxKeyword);
                return member.Invoke(new object[] { cell.Value });
            }

            // lookup FlowKeywords
            return keywords[memberName.Name](instance.GetValue<FlowInterpreter>(), (Parse)cell.Value);
        }

        TypedValue AbandonStoryTest(FlowInterpreter fixture, Parse theCells) {
            Processor.TestStatus.MarkIgnore(theCells);
            Processor.TestStatus.IsAbandoned = true;
            throw new AbandonStoryTestException();
        }

        static TypedValue Calculate(FlowInterpreter fixture, Parse theCells) {
            return new TypedValue(new CalculateFixture(fixture.SystemUnderTest ?? fixture));
        }

        static TypedValue Ignored(FlowInterpreter fixture, Parse cells) {
            return new TypedValue(new Fixture());
        }

        static Maybe<RuntimeMember> FindMember(object instance, MemberName memberName, Type type) {
            return MemberQuery.FindDirectInstance(instance,
                    new MemberSpecification(memberName).WithParameterTypes(new[] {type}));
        }
        readonly Dictionary<string, Func<FlowInterpreter, Parse, TypedValue>> keywords = new Dictionary<string, Func<FlowInterpreter, Parse, TypedValue>>();
    }
}