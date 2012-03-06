// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class FindMemberPattern: CellOperator, FindMemberOperator<Cell> {
        public bool CanFindMember(TypedValue instance, MemberQuery query) {
            return true;
        }

        public TypedValue FindMember(TypedValue instance, MemberQuery query) {
            var member = query.FindMatchingMember(instance.Type, new PatternMemberMatcher(instance.Value)) 
                    ?? query.FindMember(instance.Value);
            return new TypedValue(member, typeof(RuntimeMember));
        }

        class PatternMemberMatcher: MemberMatcher {
            public PatternMemberMatcher(object instance) {
                this.instance = instance;
            }

            public bool IsMatch(MemberName memberName, MemberInfo memberInfo) {
                if (!Attribute.IsDefined(memberInfo, typeof(MemberPatternAttribute), true)) return false;
                var attribute = (MemberPatternAttribute)Attribute.GetCustomAttribute(memberInfo, typeof (MemberPatternAttribute), true);
                var match = new Regex(attribute.Pattern).Match(memberName.OriginalName);
                if (!match.Success) return false;
                var runtimeMemberFactory = RuntimeMemberFactory.MakeFactory(memberName, memberInfo);

                var parameters = new object[match.Groups.Count - 1];
                for (var i = 0; i < parameters.Length; i++) parameters[i] = match.Groups[i+1].Value;

                RuntimeMember = new PatternRuntimeMember(runtimeMemberFactory.MakeMember(instance), parameters);
                return true;
            }

            public RuntimeMember RuntimeMember { get; private set; }

            readonly object instance;
        }

        class PatternRuntimeMember: RuntimeMember {
            public PatternRuntimeMember(RuntimeMember baseMember, object[] patternParameters) {
                this.baseMember = baseMember;
                this.patternParameters = patternParameters;
            }

            public TypedValue Invoke(object[] parameters) {
                return baseMember.Invoke(patternParameters);
            }

            public bool MatchesParameterCount(int count) {
                return baseMember.MatchesParameterCount(count);
            }

            public Type GetParameterType(int index) {
                return baseMember.GetParameterType(index);
            }

            public string GetParameterName(int index) {
                return baseMember.GetParameterName(index);
            }

            public Type ReturnType {
                get { return baseMember.ReturnType; }
            }

            public string Name {
                get { return baseMember.Name; }
            }

            readonly RuntimeMember baseMember;
            readonly object[] patternParameters;
        }
    }
}
