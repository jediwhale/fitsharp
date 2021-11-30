// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class ExtensionMemberMatcher: MemberMatcher {
        public ExtensionMemberMatcher(MemberSpecification specification, object instance) {
            this.specification = specification;
            this.instance = instance;
        }
        public Maybe<RuntimeMember> Match(IEnumerable<MemberInfo> members) {
            foreach (var member in members) {
                var methodInfo = member as MethodInfo;
                if (methodInfo == null) continue;
                if (!specification.MatchesExtension(methodInfo, instance)) continue;
                return Maybe<RuntimeMember>.Of(new ExtensionMember(methodInfo, instance));
            }
            return Maybe<RuntimeMember>.Nothing;
        }
        
        readonly MemberSpecification specification;
        readonly object instance;
    }
}