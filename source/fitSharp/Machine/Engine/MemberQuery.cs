// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class MemberQuery {
        public static Maybe<RuntimeMember> FindInstance(Func<TypedValue, MemberQuery, TypedValue> finder, TypedValue instance, MemberSpecification specification) {
            return new MemberQuery(specification, finder).Find(instance);
        }

        public static Maybe<RuntimeMember> FindDirectInstance(object instance, MemberSpecification specification) {
            return new MemberQuery(specification).FindMember(TypedValue.Of(instance));
        }

        public static RuntimeMember GetDirectInstance(object instance, MemberSpecification specification) {
            foreach (var member in FindDirectInstance(instance, specification).Value) {
                return member;
            }
            throw specification.MemberMissingException(instance.GetType());
        }

        public static TypedValue FindMember(TypedValue instance, MemberQuery query) {
            return query.FindMember(instance).TypedValue;
        }

        public MemberQuery(MemberSpecification specification): this(specification, FindMember) {}

        public MemberQuery StaticOnly() {
            flags = BindingFlags.Static;
            return this;
        }

        public MemberSpecification Specification { get; }

        public Maybe<RuntimeMember> FindMember(TypedValue instance) {
            return Specification.FindMatchingMember(this, instance);
        }

        public Maybe<RuntimeMember> FindInstanceMember(TypedValue instance) {
            var targetType = instance.Type.IsAssignableTo(typeof(Type)) ? (Type)instance.Value : instance.Type;
            return FindBasicMember(instance.Value, targetType)
                    .OrMaybe(() => FindIndexerMember(instance.Value, targetType));
        }

        public Maybe<RuntimeMember> FindExtensionMember(Type extensionType, TypedValue instance) {
            return FindMatchingMember(extensionType, new ExtensionMemberMatcher(Specification, instance.Value));
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
        
        Maybe<RuntimeMember> Find(TypedValue instance) {
            var target = instance;
            while (true) {
                var member = finder(target, this);
                if (member.HasValue) return Maybe<RuntimeMember>.Of(member.GetValue<RuntimeMember>());

                if (!typeof(DomainAdapter).IsAssignableFrom(target.Type)) break;
                
                var adapter = target.Value as DomainAdapter;
                var sut = adapter?.SystemUnderTest;
                if (sut == null || sut == adapter) break;
                target = new TypedValue(sut);
            }
            return Maybe<RuntimeMember>.Nothing;
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
