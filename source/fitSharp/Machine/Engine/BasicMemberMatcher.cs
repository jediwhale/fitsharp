// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class BasicMemberMatcher: MemberMatcher {
        public BasicMemberMatcher(object instance, MemberName memberName, int parameterCount, IList<Type> parameterTypes, IList<IdentifierName> parameterIdNames) {
            this.instance = instance;
            this.memberName = memberName;
            this.parameterIdNames = parameterIdNames;
            this.parameterTypes = parameterTypes;
            this.parameterCount = parameterCount;
        }

        public Maybe<RuntimeMember> Match(IEnumerable<MemberInfo> members) {
            foreach (var memberInfo in members) {
                var runtimeMemberFactory = RuntimeMemberFactory.MakeFactory(memberName, memberInfo);
                if (!runtimeMemberFactory.Matches(memberName)) continue;
                var runtimeMember = runtimeMemberFactory.MakeMember(instance);
                if (!Matches(runtimeMember)) continue;
                return new Maybe<RuntimeMember>(runtimeMember);
            }
            return Maybe<RuntimeMember>.Nothing;
        }

        bool Matches(RuntimeMember runtimeMember) {
            if (!runtimeMember.MatchesParameterCount(parameterCount)) return false;
            if (parameterTypes != null) {
                for (int i = 0; i < parameterCount; i++) {
                    if (runtimeMember.GetParameterType(i) != parameterTypes[i]) return false;
                }
            }
            return
                parameterIdNames == null ||
                parameterIdNames.All(name => HasMatchingParameter(runtimeMember, name));
        }

        bool HasMatchingParameter(RuntimeMember runtimeMember, NameMatcher name) {
            for (var i = 0; i < parameterCount; i++) {
                if (name.Matches(runtimeMember.GetParameterName(i))) return true;
            }
            return false;
        }

        readonly object instance;
        readonly MemberName memberName;
        readonly int parameterCount;
        readonly IList<Type> parameterTypes;
        readonly IList<IdentifierName> parameterIdNames;
    }
}
