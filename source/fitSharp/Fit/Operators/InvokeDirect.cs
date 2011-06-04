// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
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
                var type = Processor.ParseString<Cell, Type>("fit.Parse");
                var member = RuntimeType.FindDirectInstance(instance.Value, new IdentifierName(memberName), new[] {type});
                return member != null
                             ? member.Invoke(new object[] {parameters.Value})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 1));
            }
        }
    }
}
