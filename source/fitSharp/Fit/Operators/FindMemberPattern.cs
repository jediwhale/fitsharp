// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class FindMemberPattern: CellOperator, FindMemberOperator<Cell> {
        public bool CanFindMember(TypedValue instance, MemberQuery query) {
            return true;
        }

        public TypedValue FindMember(TypedValue instance, MemberQuery query) {
            var member = query.FindMatchingMember(instance.Type, new PatternMemberMatcher(Processor, instance.Value, query.Specification)) 
                    .OrMaybe(() => query.FindMember(instance));
            return member.TypedValue;
        }

        class PatternMemberMatcher: MemberMatcher {
            public PatternMemberMatcher(CellProcessor processor, object instance, MemberSpecification specification) {
                this.processor = processor;
                this.specification = specification;
                this.instance = instance;
            }

            public Maybe<RuntimeMember> Match(IEnumerable<MemberInfo> members) {
                foreach (var memberInfo in members) {
                    if (!Attribute.IsDefined(memberInfo, typeof(MemberPatternAttribute), true)) continue;
                    var attribute = (MemberPatternAttribute)Attribute.GetCustomAttribute(memberInfo, typeof (MemberPatternAttribute), true);
                    var match = specification.MatchRegexName(attribute.Pattern);
                    if (!match.Success) continue;
                    var runtimeMemberFactory = RuntimeMemberFactory.MakeFactory(specification, memberInfo);

                    var parameters = new string[match.Groups.Count - 1];
                    foreach (var i in parameters.Length.Count()) parameters[i] = match.Groups[i+1].Value;

                    return new Maybe<RuntimeMember>(new PatternRuntimeMember(processor, runtimeMemberFactory.MakeMember(instance), parameters));
                }
                return Maybe<RuntimeMember>.Nothing;
            }

            readonly CellProcessor processor;
            readonly object instance;
            readonly MemberSpecification specification;
        }

        class PatternRuntimeMember: RuntimeMember {
            public PatternRuntimeMember(CellProcessor processor, RuntimeMember baseMember, string[] patternParameters) {
                this.baseMember = baseMember;
                this.processor = processor;
                this.patternParameters = patternParameters;
            }

            public TypedValue Invoke(object[] invokeParameters) {
                if (!baseMember.MatchesParameterCount(patternParameters.Length)) {
                    throw new InvalidMethodException(
                        $"Member pattern for {baseMember.Name} has {patternParameters.Length} parameters.");
                }
                var parameters = new object[patternParameters.Length];
                foreach (var i in patternParameters.Length.Count()) {
                    parameters[i] = processor.ParseString(GetParameterType(i), patternParameters[i]).Value;
                }
                return baseMember.Invoke(parameters);
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

            public Type ReturnType => baseMember.ReturnType;
            public string Name => baseMember.Name;

            readonly RuntimeMember baseMember;
            readonly string[] patternParameters;
            readonly CellProcessor processor;
        }
    }
}
