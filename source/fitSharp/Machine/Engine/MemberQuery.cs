// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
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

        public static Maybe<RuntimeMember> FindDirectInstance(object instance, MemberName memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount).FindMember(instance);
        }

        public static RuntimeMember GetDirectInstance(object instance, MemberName memberName, int parameterCount) {
            foreach (var member in new MemberQuery(memberName, parameterCount).FindMember(instance).Value) {
                return member;
            }
            throw new MemberMissingException(instance.GetType(), memberName.Name, parameterCount);
        }

        public static Maybe<RuntimeMember> FindDirectInstance(object instance, MemberName memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length)
                .WithParameterTypes(parameterTypes)
                .FindMember(instance);
        }

        public MemberQuery(MemberName memberName, int parameterCount) {
            this.memberName = memberName;
            specification = new MemberSpecification(memberName, parameterCount);
            flags = BindingFlags.Instance | BindingFlags.Static;
            finder = FindMember;
        }

        public MemberQuery WithParameterTypes(IList<Type> parameterTypes) {
            specification.WithParameterTypes(parameterTypes);
            return this;
        }

        public MemberQuery WithParameterNames(IEnumerable<string> parameterNames) {
            specification.WithParameterNames(parameterNames);
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

        public MemberSpecification Specification { get { return specification; } }
        public MemberName MemberName { get { return memberName; } }

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
            return query.FindMember(instance.Value).TypedValue;
        }

        public Maybe<RuntimeMember> FindMember(object instance) {
            var targetType = instance as Type ?? instance.GetType();
            return FindBasicMember(instance, targetType)
                    .OrMaybe(() => FindIndexerMember(instance, targetType));
        }

        public Maybe<RuntimeMember> FindMatchingMember(Type targetType, MemberMatcher matcher) {
            return FindMatchingMember(targetType, BindingFlags.Public, matcher)
                    .OrMaybe(() => FindMatchingMember(targetType, BindingFlags.NonPublic, matcher));
        }

        Maybe<RuntimeMember> FindBasicMember(object instance, Type targetType) {
            var matcher = new BasicMemberMatcher(instance, memberName, specification);
            return FindMatchingMember(targetType, matcher);
        }

        Maybe<RuntimeMember> FindIndexerMember(object instance, Type targetType) {
            var matcher = new IndexerMemberMatcher(instance, memberName, specification);
            return FindMatchingMember(targetType, matcher);
        }

        Maybe<RuntimeMember> FindMatchingMember(Type targetType, BindingFlags accessFlag, MemberMatcher matcher) {
            return matcher.Match(targetType.GetMembers(flags | accessFlag | BindingFlags.FlattenHierarchy));
        }

        readonly MemberName memberName;
        readonly MemberSpecification specification;

        Func<TypedValue, MemberQuery, TypedValue> finder;
        BindingFlags flags;
    }
}
