// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class InvokeDirect: CellOperator, InvokeOperator<Cell> {
        public const string SetUpMethod = DirectPrefix + "setup";
        public const string TearDownMethod = DirectPrefix + "teardown";

        public static string MakeDirect(string memberName) { return DirectPrefix + memberName; }

        const string DirectPrefix = ":";

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return memberName.StartsWith(DirectPrefix);
        }

        public TypedValue Invoke(TypedValue instance, string directMemberName, Tree<Cell> parameters) {
            var memberName = directMemberName.Substring(DirectPrefix.Length);
            if (directMemberName == SetUpMethod || directMemberName == TearDownMethod) {
                var member = RuntimeType.FindDirectInstance(instance.Value, new IdentifierName(memberName), 0);
                return member != null
                             ? member.Invoke(new object[] {})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 0));
            }
            else {
                var member = RuntimeType.FindDirectInstance(instance.Value, new IdentifierName(memberName), new[] {typeof (Parse)});
                return member != null
                             ? member.Invoke(new object[] {parameters.Value})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 1));
            }
        }
    }
}
