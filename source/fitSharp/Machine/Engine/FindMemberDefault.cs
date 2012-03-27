// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class FindMemberDefault<T, P>: Operator<T, P>, FindMemberOperator<T> where P: class, Processor<T> {
        public bool CanFindMember(TypedValue instance, MemberQuery query) {
            return true;
        }

        public TypedValue FindMember(TypedValue instance, MemberQuery query) {
            return query.FindMember(instance.Value).TypedValue;
        }
    }
}
