// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeSpecialAction: CellOperator, InvokeOperator<Cell> {


        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return memberName.IsSpecialAction;
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            var type = Processor.ApplicationUnderTest.FindType("fit.Parse").Type;

    		// lookup Fixture
			foreach (var member in FindMember(instance, memberName, type).Value)
			{
                return member.Invoke(new object[] { parameters.Value });
            }

			// lookup FlowKeywords
			var runtimeType = Processor.ApplicationUnderTest.FindType("fit.Fixtures.FlowKeywords");
			var runtimeMember = runtimeType.GetConstructor(2);
			var flowKeywords = runtimeMember.Invoke(new[] { instance.Value, Processor });

			foreach (var member in FindMember(flowKeywords, memberName, type).Value)
			{
				return member.Invoke(new object[] { parameters.Value });
			}
			
			return TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName.Name, 1));
        }

        static Maybe<RuntimeMember> FindMember(TypedValue flowKeywords, MemberName memberName, Type type) {
            return MemberQuery.FindDirectInstance(flowKeywords.Value,
                    new MemberSpecification(memberName).WithParameterTypes(new[] {type}));
        }
    }
}
