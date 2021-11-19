// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;

namespace fitSharp.Machine.Engine {
    public abstract class RuntimeMemberFactory {
        public static RuntimeMemberFactory MakeFactory(MemberSpecification specification, MemberInfo memberInfo) {
            switch (memberInfo.MemberType) {
                case MemberTypes.Method:
                    return new MethodMemberFactory(memberInfo, specification);
                case MemberTypes.Field:
                    return new FieldMemberFactory(memberInfo);
                case MemberTypes.Property:
                    return new PropertyMemberFactory(memberInfo);
                case MemberTypes.Constructor:
                    return new ConstructorMemberFactory(memberInfo);
                default:
                    return new UnsupportedMemberFactory(memberInfo);
            }
        }

        public virtual bool Matches(MemberSpecification specification) {
            return specification.MatchesGetSetName(info.Name);
        }

        public abstract RuntimeMember MakeMember(object instance);

        protected RuntimeMemberFactory(MemberInfo info) {
            this.info = info;
        }

        protected readonly MemberInfo info;
    }

    public class MethodMemberFactory: RuntimeMemberFactory {
        public MethodMemberFactory(MemberInfo info, MemberSpecification specification) : base(info) {
            this.specification = specification;
        }

        public override bool Matches(MemberSpecification specification) {
            return specification.Matches(Info);
        }

        public override RuntimeMember MakeMember(object instance) {
            return specification.MakeMember(Info, instance);
        }

        MethodInfo Info => (MethodInfo) info;

        readonly MemberSpecification specification;
    }

    public class PropertyMemberFactory: RuntimeMemberFactory {
        public PropertyMemberFactory(MemberInfo info) : base(info) {}
        public override RuntimeMember MakeMember(object instance) { return new PropertyMember(info, instance); }
    }

    public class FieldMemberFactory: RuntimeMemberFactory {
        public FieldMemberFactory(MemberInfo info) : base(info) {}
        public override RuntimeMember MakeMember(object instance) { return new FieldMember(info, instance); }
    }

    public class ConstructorMemberFactory: RuntimeMemberFactory {
        public ConstructorMemberFactory(MemberInfo info) : base(info) {}
        public override RuntimeMember MakeMember(object instance) { return new ConstructorMember(info, instance); }
    }

    public class UnsupportedMemberFactory: RuntimeMemberFactory {
        public UnsupportedMemberFactory(MemberInfo info) : base(info) {}

        public override bool Matches(MemberSpecification specification) { return false; }

        public override RuntimeMember MakeMember(object instance) { throw new InvalidOperationException(); }
    }
}
