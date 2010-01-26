// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public interface MemberQueryable {
        RuntimeMember Find(IdentifierName memberName, int parameterCount, Type[] parameterTypes);
    }

    public class RuntimeType {

        public Type Type { get; private set; }

        public RuntimeType(Type type) {
            Type = type;
        }

        public RuntimeMember FindStatic(string memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length, BindingFlags.Static, parameterTypes).Find(Type);
        }

        public static RuntimeMember GetInstance(TypedValue instance, string memberName, int parameterCount) {
            RuntimeMember runtimeMember = FindInstance(instance.Value, memberName, parameterCount);
            if (runtimeMember == null) throw new MemberMissingException(instance.Type, memberName, parameterCount);
            return runtimeMember;
        }

        public RuntimeMember GetConstructor(int parameterCount) {
            RuntimeMember runtimeMember = FindInstance(Type, ".ctor", parameterCount);
            if (runtimeMember == null) throw new ConstructorMissingException(Type, parameterCount);
            return runtimeMember;
        }

        public RuntimeMember FindConstructor(Type[] parameterTypes) {
            return FindInstance(Type, ".ctor", parameterTypes);
        }

        public static RuntimeMember FindInstance(object instance, string memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount, BindingFlags.Instance | BindingFlags.Static, null).Find(instance);
        }

        public static RuntimeMember FindDirectInstance(object instance, string memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount, BindingFlags.Instance | BindingFlags.Static, null).FindMember(instance);
        }

        public static RuntimeMember FindInstance(object instance, string memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length, BindingFlags.Instance | BindingFlags.Static, parameterTypes).Find(instance);
        }

        public static RuntimeMember FindDirectInstance(object instance, string memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length, BindingFlags.Instance | BindingFlags.Static, parameterTypes).FindMember(instance);
        }

        public TypedValue CreateInstance() {
            return new TypedValue(Type.Assembly.CreateInstance(Type.FullName), Type);
        }

        private class MemberQuery {
            private readonly IdentifierName memberName;
            private readonly int parameterCount;
            private readonly BindingFlags flags;
            private readonly Type[] parameterTypes;

            public MemberQuery(string memberName, int parameterCount, BindingFlags flags, Type[] parameterTypes) {
                this.memberName = new IdentifierName(memberName);
                this.parameterCount = parameterCount;
                this.flags = flags;
                this.parameterTypes = parameterTypes;
            }

            public RuntimeMember Find(object instance) {
                object target = instance;
                while (target != null) {
                    var queryable = target as MemberQueryable;
                    if (queryable != null) {
                        RuntimeMember queryableMember = queryable.Find(memberName, parameterCount, parameterTypes);
                        if (queryableMember != null) return queryableMember;
                    }

                    RuntimeMember member = FindMember(target);
                    if (member != null) return member;

                    var adapter = target as DomainAdapter;
                    if (adapter == null) break;

                    target = adapter.SystemUnderTest;
                    if (target == adapter) break;
                }
                return null;
            }

            public RuntimeMember FindMember(object instance) {
                var targetType = instance as Type ?? instance.GetType();
                return FindMemberByName(instance, targetType) ?? FindIndexerMember(instance, targetType);
            }

            private RuntimeMember FindMemberByName(object instance, Type targetType) {
                return FindMemberByName(instance, targetType, BindingFlags.Public)
                       ?? FindMemberByName(instance, targetType, BindingFlags.NonPublic);
            }

            private RuntimeMember FindMemberByName(object instance, Type targetType, BindingFlags accessFlag) {
                foreach (MemberInfo memberInfo in targetType.GetMembers(flags | accessFlag | BindingFlags.FlattenHierarchy)) {
                    if (!MatchesName(memberInfo.Name)) continue;
                    RuntimeMember runtimeMember = MakeMember(memberInfo, instance);
                    if (Matches(runtimeMember)) return runtimeMember;
                }
                return null;
            }

            private bool MatchesName(string name) {
                if (memberName.Matches(name)) return true;
                if (!memberName.MatchName.StartsWith("set") && !memberName.MatchName.StartsWith("get")) return false;
                return new IdentifierName(memberName.MatchName.Substring(3)).Matches(name);
            }

            private static RuntimeMember MakeMember(MemberInfo memberInfo, object instance) {
                switch (memberInfo.MemberType) {
                    case MemberTypes.Method:
                        return new MethodMember(memberInfo, instance);
                    case MemberTypes.Field:
                        return new FieldMember(memberInfo, instance);
                    case MemberTypes.Property:
                        return new PropertyMember(memberInfo, instance);
                    case MemberTypes.Constructor:
                        return new ConstructorMember(memberInfo, instance);
                    default:
                        throw new NotImplementedException(string.Format("Member type {0} not supported",
                                                                        memberInfo.MemberType));
                }
            }

            private bool Matches(RuntimeMember runtimeMember) {
                if (!runtimeMember.MatchesParameterCount(parameterCount)) return false;
                if (parameterTypes == null) return true;
                for (int i = 0; i < parameterCount; i++) {
                    if (runtimeMember.GetParameterType(i) != parameterTypes[i]) return false;
                }
                return true;
            }

            private RuntimeMember FindIndexerMember(object instance, Type targetType) {
                if (parameterCount != 0) return null;
                foreach (MemberInfo memberInfo in targetType.GetMembers(flags | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)) {
                    if (memberInfo.Name != "get_Item") continue;
                    RuntimeMember indexerMember = new IndexerMember(memberInfo, instance, memberName.SourceName);
                    if (indexerMember.MatchesParameterCount(1) && indexerMember.GetParameterType(0) == typeof(string)) {
                        return indexerMember;
                    }
                }
                return null;
            }
        }
    }
}