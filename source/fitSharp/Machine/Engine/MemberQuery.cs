// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public interface MemberQueryable {
        RuntimeMember Find(MemberQuery query);
    }


    public class MemberQuery {
        public static RuntimeMember FindInstance(Func<TypedValue, MemberQuery, TypedValue> finder, object instance, MemberName memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount).Using(finder).Find(instance);
        }

        public static RuntimeMember FindInstance(Func<TypedValue, MemberQuery, TypedValue> finder, object instance, MemberName memberName, IList<string> parameterNames) {
            return new MemberQuery(memberName, parameterNames.Count)
                .WithParameterNames(parameterNames)
                .Using(finder)
                .Find(instance);
        }

        public static RuntimeMember FindDirectInstance(object instance, MemberName memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount).FindMember(instance);
        }

        public static RuntimeMember FindDirectInstance(object instance, MemberName memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length)
                .WithParameterTypes(parameterTypes)
                .FindMember(instance);
        }

        public MemberQuery(MemberName memberName, int parameterCount) {
            this.memberName = memberName;
            this.parameterCount = parameterCount;
            flags = BindingFlags.Instance | BindingFlags.Static;
            finder = FindMember;
        }

        public MemberQuery WithParameterTypes(IList<Type> parameterTypes) {
            this.parameterTypes = parameterTypes;
            return this;
        }

        public MemberQuery WithParameterNames(IEnumerable<string> parameterNames) {
            parameterIdNames = new List<IdentifierName>();
            foreach (string name in parameterNames) {
                parameterIdNames.Add(new IdentifierName(name));
            }
            return this;
        }

        public MemberQuery StaticOnly() {
            flags = BindingFlags.Static;
            return this;
        }

        public MemberQuery Using(Func<TypedValue, MemberQuery, TypedValue> finder) {
            this.finder = finder;
            return this;
        }

        public IdentifierName IdentifierName { get { return new IdentifierName(memberName.Name); } }
        public bool IsGetter { get { return parameterCount == 0; } }
        public bool IsSetter { get { return parameterCount == 1; } }
        public override string ToString() { return memberName.ToString(); }

        public RuntimeMember Find(object instance) {
            object target = instance;
            while (target != null) {
                var member = finder(new TypedValue(target), this);
                if (member.HasValue) return member.GetValue<RuntimeMember>();


                var adapter = target as DomainAdapter;
                if (adapter == null) break;

                target = adapter.SystemUnderTest;
                if (target == adapter) break;
            }
            return null;
        }

        public static TypedValue FindMember(TypedValue instance, MemberQuery query) {
            return new TypedValue(query.FindMember(instance.Value), typeof(RuntimeMember));
        }

        public RuntimeMember FindMember(object instance) {
            var targetType = instance as Type ?? instance.GetType();
            var matcher = new BasicMemberMatcher(instance, parameterCount, parameterTypes, parameterIdNames);
            return FindMatchingMember(targetType, matcher) ?? FindIndexerMember(instance, targetType);
        }

        public RuntimeMember FindMatchingMember(Type targetType, MemberMatcher matcher) {
            return FindMatchingMember(targetType, BindingFlags.Public, matcher)
                   ?? FindMatchingMember(targetType, BindingFlags.NonPublic, matcher);
        }

        RuntimeMember FindMatchingMember(Type targetType, BindingFlags accessFlag, MemberMatcher matcher) {
            foreach (var memberInfo in targetType.GetMembers(flags | accessFlag | BindingFlags.FlattenHierarchy)) {
                if (!matcher.IsMatch(memberName, memberInfo)) continue;
                return matcher.RuntimeMember;

            }
            return null;
        }

        RuntimeMember FindIndexerMember(object instance, Type targetType) {
            if (parameterCount != 0) return null;
            foreach (MemberInfo memberInfo in targetType.GetMembers(flags | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)) {
                if (memberInfo.Name != "get_Item") continue;
                RuntimeMember indexerMember = new IndexerMember(memberInfo, instance, memberName.Name);
                if (indexerMember.MatchesParameterCount(1) && indexerMember.GetParameterType(0) == typeof(string)) {
                    return indexerMember;
                }
            }
            return null;
        }

        readonly MemberName memberName;
        readonly int parameterCount;

        Func<TypedValue, MemberQuery, TypedValue> finder;
        BindingFlags flags;
        IList<Type> parameterTypes;
        IList<IdentifierName> parameterIdNames;
    }
}