// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class MemberQuery {
        public static RuntimeMember FindInstance(Func<TypedValue, MemberQuery, TypedValue> finder, object instance, MemberSpecification specification) {
            return new MemberQuery(specification, finder).Find(instance);
        }

        public static Maybe<RuntimeMember> FindDirectInstance(object instance, MemberSpecification specification) {
            return new MemberQuery(specification).FindMember(instance);
        }

        public static RuntimeMember GetDirectInstance(object instance, MemberSpecification specification) {
            foreach (var member in FindDirectInstance(instance, specification).Value) {
                return member;
            }
            throw specification.MemberMissingException(instance.GetType());
        }

        public static TypedValue FindMember(TypedValue instance, MemberQuery query) {
            return query.FindMember(instance.Value).TypedValue;
        }

        public MemberQuery(MemberSpecification specification): this(specification, FindMember) {}

        public MemberQuery StaticOnly() {
            flags = BindingFlags.Static;
            return this;
        }

        public MemberSpecification Specification { get; }

        public Maybe<RuntimeMember> FindMember(object instance) {
            return Specification.FindMatchingMember(this, instance);
        }

        public Maybe<RuntimeMember> FindInstanceMember(object instance) {
            var targetType = instance as Type ?? instance.GetType(); 
            return FindBasicMember(instance, targetType)
                    .OrMaybe(() => FindIndexerMember(instance, targetType));
        }

        public Maybe<RuntimeMember> FindExtensionMember(Type extensionType, object instance) {
            return FindMatchingMember(extensionType, new ExtensionMemberMatcher(Specification, instance));
        }

        public Maybe<RuntimeMember> FindMatchingMember(Type targetType, MemberMatcher matcher) {
            return FindMatchingMember(targetType, BindingFlags.Public, matcher)
                    .OrMaybe(() => FindMatchingMember(targetType, BindingFlags.NonPublic, matcher));
        }

        MemberQuery(MemberSpecification specification, Func<TypedValue, MemberQuery, TypedValue> finder) {
            Specification = specification;
            flags = BindingFlags.Instance | BindingFlags.Static;
            this.finder = finder;
        }
        
        RuntimeMember Find(object instance) {
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
        
        Maybe<RuntimeMember> FindBasicMember(object instance, Type targetType) {
            var matcher = new BasicMemberMatcher(instance, Specification);
            return FindMatchingMember(targetType, matcher);
        }

        Maybe<RuntimeMember> FindIndexerMember(object instance, Type targetType) {
            var matcher = new IndexerMemberMatcher(instance, Specification);
            return FindMatchingMember(targetType, matcher);
        }

        Maybe<RuntimeMember> FindMatchingMember(IReflect targetType, BindingFlags accessFlag, MemberMatcher matcher) {
            return matcher.Match(targetType.GetMembers(flags | accessFlag | BindingFlags.FlattenHierarchy));
        }

        readonly Func<TypedValue, MemberQuery, TypedValue> finder;
        BindingFlags flags;
    }
}
