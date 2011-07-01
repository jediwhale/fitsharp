// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeSpecialAction: CellOperator, InvokeOperator<Cell> {

        public static string MakeName(string memberName) { return Prefix + memberName; }

        const string Prefix = ":";

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return memberName.StartsWith(Prefix);
        }

        public TypedValue Invoke(TypedValue instance, string prefixedMemberName, Tree<Cell> parameters) {
            var memberName = prefixedMemberName.Substring(Prefix.Length);
            var type = Processor.ApplicationUnderTest.FindType("fit.Parse").Type;

            var runtimeType = Processor.ApplicationUnderTest.FindType("fit.Fixtures.FlowKeywords");
            var runtimeMember = runtimeType.GetConstructor(1);
            var flowKeywords = runtimeMember.Invoke(new [] {instance.Value});
            var member = RuntimeType.FindDirectInstance(flowKeywords.Value, new IdentifierName(memberName), new[] {type});
            if (member != null)
                return member.Invoke(new object[] {parameters.Value});

            member = RuntimeType.FindDirectInstance(instance.Value, new IdentifierName(memberName), new[] {type});
            return member != null
                            ? member.Invoke(new object[] {parameters.Value})
                            : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 1));
        }
    }
}
