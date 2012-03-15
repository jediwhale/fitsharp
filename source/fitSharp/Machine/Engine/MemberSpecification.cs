// Copyright © 2012 Syterra Software Inc. All rights reserved.
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

        public MemberSpecification WithParameterTypes(IList<Type> parameterTypes) {
            this.parameterTypes = parameterTypes;
            parameterCount = parameterTypes.Count;
            return this;
        }

        public MemberSpecification WithParameterNames(IEnumerable<string> parameterNames) {
            parameterCount = parameterNames.Count();
            parameterIdNames = new List<IdentifierName>();
            foreach (var name in parameterNames) {
                parameterIdNames.Add(new IdentifierName(name));
            }
            return this;
        }

        public bool IsGetter { get { return parameterCount == 0; } }
        public bool IsSetter { get { return parameterCount == 1; } }
        public string MemberName { get { return memberName.Name; } }
        public override string ToString() { return memberName.ToString(); }

        public bool MatchesIdentifierName(string name) {
            return new IdentifierName(memberName.Name).Matches(name);
        }

        public bool MatchesBaseName(string name) {
            return new IdentifierName(memberName.BaseName).Matches(name);
        }

        public Match MatchRegexName(string pattern) {
            return new Regex(pattern).Match(memberName.OriginalName);
        }

        public bool MatchesGetSetName(string name) {
            var identifier = new IdentifierName(memberName.Name);
            if (identifier.Matches(name)) return true;
            if (!identifier.MatchName.StartsWith("set") && !identifier.MatchName.StartsWith("get")) return false;
            return new IdentifierName(identifier.MatchName.Substring(3)).Matches(name);
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

        bool HasMatchingParameter(RuntimeMember runtimeMember, NameMatcher name) {
            for (var i = 0; i < parameterCount; i++) {
                if (name.Matches(runtimeMember.GetParameterName(i))) return true;
            }
            return false;
        }

        public MethodInfo MakeGenericMethod(MethodInfo baseMethod) {
            return baseMethod.MakeGenericMethod(memberName.GenericTypes);
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
