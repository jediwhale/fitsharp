// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeSetUpTearDown: CellOperator, InvokeOperator<Cell> {
        public const string SetUpMethod = "#setup";
        public const string TearDownMethod = "#teardown";

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return memberName == SetUpMethod || memberName == TearDownMethod;
        }

        public TypedValue Invoke(TypedValue instance, string prefixedMemberName, Tree<Cell> parameters) {
            var memberName = prefixedMemberName.Substring(1);
            var member = RuntimeType.FindDirectInstance(instance.Value, new IdentifierName(memberName), 0);
            return member != null
                            ? member.Invoke(new object[] {})
                            : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 0));
        }
    }
}
