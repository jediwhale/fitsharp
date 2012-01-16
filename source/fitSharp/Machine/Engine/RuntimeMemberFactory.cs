// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public abstract class RuntimeMemberFactory {
        public static RuntimeMemberFactory MakeFactory(MemberName memberName, MemberInfo memberInfo) {
            switch (memberInfo.MemberType) {
                case MemberTypes.Method:
                    return new MethodMemberFactory(memberInfo, memberName.GenericTypes);
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

        public virtual bool Matches(MemberName memberName) {
            var identifier = new IdentifierName(memberName.Name);
            if (identifier.Matches(info.Name)) return true;
            if (!identifier.MatchName.StartsWith("set") && !identifier.MatchName.StartsWith("get")) return false;
            return new IdentifierName(identifier.MatchName.Substring(3)).Matches(info.Name);
        }

        public abstract RuntimeMember MakeMember(object instance);

        protected RuntimeMemberFactory(MemberInfo info) {
            this.info = info;
        }

        protected MemberInfo info;
    }

    public class MethodMemberFactory: RuntimeMemberFactory {
        public MethodMemberFactory(MemberInfo info, Type[] genericTypes) : base(info) {
            this.genericTypes = genericTypes;
        }

        public override bool Matches(MemberName memberName) {
            return Info.IsGenericMethod
                ? new IdentifierName(memberName.BaseName).Matches(info.Name)
                : base.Matches(memberName);
        }

        public override RuntimeMember MakeMember(object instance) {
            return Info.IsGenericMethod
                       ? new MethodMember(Info.MakeGenericMethod(genericTypes), instance)
                       : new MethodMember(info, instance);
        }

        MethodInfo Info { get { return (MethodInfo) info; } }

        readonly Type[] genericTypes;
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

        public override bool Matches(MemberName memberName) { return false; }

        public override RuntimeMember MakeMember(object instance) { throw new InvalidOperationException(); }
    }
}
