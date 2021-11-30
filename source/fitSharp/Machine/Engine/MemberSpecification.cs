// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class MemberSpecification {
        public MemberSpecification(MemberName memberName, int parameterCount) {
            this.parameterCount = parameterCount;
            this.memberName = memberName;
        }

        public MemberSpecification(string memberName, int parameterCount) {
            this.parameterCount = parameterCount;
            this.memberName = new MemberName(memberName);
        }

        public MemberSpecification(MemberName memberName) {
            parameterCount = 0;
            this.memberName = memberName;
        }

        public MemberSpecification WithParameterTypes(IList<Type> types) {
            parameterTypes = types;
            parameterCount = types.Count;
            return this;
        }

        public MemberSpecification WithParameterNames(IEnumerable<string> parameterNames) {
            parameterIdNames = new List<IdentifierName>();
            foreach (var name in parameterNames) {
                parameterIdNames.Add(new IdentifierName(name));
            }
            parameterCount = parameterIdNames.Count;
            return this;
        }

        public bool IsGetter => parameterCount == 0;
        public bool IsSetter => parameterCount == 1;
        public string MemberName => memberName.Name;
        public override string ToString() { return memberName.ToString(); }

        public bool MatchesIdentifierName(string name) {
            return new IdentifierName(memberName.Name).Matches(name);
        }

        public Match MatchRegexName(string pattern) {
            return new Regex(pattern).Match(memberName.OriginalName);
        }

        public bool MatchesGetSetName(string name) {
            return memberName.MatchesGetSetName(name);
        }

        public bool MatchesParameterCount(RuntimeMember runtimeMember) {
            return runtimeMember.MatchesParameterCount(parameterCount);
        }

        public bool MatchesParameterTypes(RuntimeMember runtimeMember) {
            if (parameterTypes == null) return true;
            for (var i = 0; i < parameterCount; i++) {
                if (runtimeMember.GetParameterType(i) != parameterTypes[i]) return false;
            }
            return true;
        }

        public bool MatchesParameterNames(RuntimeMember runtimeMember) {
            return
                parameterIdNames == null ||
                parameterIdNames.All(name => HasMatchingParameter(runtimeMember, name));
        }

        public bool MatchesExtension(MethodInfo info, object instance) {
            return
                Matches(info) &&
                parameterCount == info.GetParameters().Length - 1 &&
                info.GetParameters()[0].ParameterType.IsInstanceOfType(instance);
        }

        public bool Matches(MethodInfo info) {
            return memberName.Matches(info);
        }

        public RuntimeMember MakeMember(MethodInfo info, object instance) {
            return memberName.MakeMember(info, instance);
        }

        public Maybe<RuntimeMember> FindMatchingMember(MemberQuery query, object instance) {
            return memberName.FindMatchingMember(query, instance);
        }

        bool HasMatchingParameter(RuntimeMember runtimeMember, NameMatcher name) {
            for (var i = 0; i < parameterCount; i++) {
                if (name.Matches(runtimeMember.GetParameterName(i))) return true;
            }
            return false;
        }

        public MemberMissingException MemberMissingException(Type type) {
            return new MemberMissingException(type, memberName.Name, parameterCount);
        }

        readonly MemberName memberName;
        int parameterCount;
        IList<Type> parameterTypes;
        IList<IdentifierName> parameterIdNames;
    }
}
