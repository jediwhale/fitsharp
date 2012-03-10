// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class IndexerMemberMatcher: MemberMatcher {
        public IndexerMemberMatcher(object instance, MemberName memberName, MemberSpecification specification) {
            this.memberName = memberName;
            this.specification = specification;
            this.instance = instance;
        }

        public Maybe<RuntimeMember> Match(IEnumerable<MemberInfo> members) {
            if (!specification.IsGetter) return Maybe<RuntimeMember>.Nothing;
            foreach (var memberInfo in members) {
                if (memberInfo.Name != "get_Item") continue;
                RuntimeMember indexerMember = new IndexerMember(memberInfo, instance, memberName.Name);
                if (indexerMember.MatchesParameterCount(1) && indexerMember.GetParameterType(0) == typeof(string)) {
                    return new Maybe<RuntimeMember>(indexerMember);
                }
            }
            return Maybe<RuntimeMember>.Nothing;
        }

        readonly object instance;
        readonly MemberName memberName;
        readonly MemberSpecification specification;
    }
}
