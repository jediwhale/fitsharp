// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public interface MemberQueryable {
        RuntimeMember Find(IdentifierName memberName, int parameterCount, IList<Type> parameterTypes);
    }

    public class RuntimeType {

        public Type Type { get; private set; }

        public RuntimeType(Type type) {
            Type = type;
        }

        public RuntimeMember FindStatic(IdentifierName memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length, BindingFlags.Static)
                .WithParameterTypes(parameterTypes)
                .Find(Type);
        }

        public static RuntimeMember GetInstance(TypedValue instance, IdentifierName memberName, int parameterCount) {
            RuntimeMember runtimeMember = FindInstance(instance.Value, memberName, parameterCount);
            if (runtimeMember == null) throw new MemberMissingException(instance.Type, memberName.SourceName, parameterCount);
            return runtimeMember;
        }

        public RuntimeMember GetConstructor(int parameterCount) {
            RuntimeMember runtimeMember = FindInstance(Type, IdentifierName.Constructor, parameterCount);
            if (runtimeMember == null) throw new ConstructorMissingException(Type, parameterCount);
            return runtimeMember;
        }

        public RuntimeMember FindConstructor(Type[] parameterTypes) {
            return FindInstance(Type, IdentifierName.Constructor, parameterTypes);
        }

        public static RuntimeMember FindInstance(object instance, IdentifierName memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount, BindingFlags.Instance | BindingFlags.Static).Find(instance);
        }

        public static RuntimeMember FindInstance(object instance, IdentifierName memberName, IList<string> parameterNames) {
            return new MemberQuery(memberName, parameterNames.Count, BindingFlags.Instance | BindingFlags.Static)
                .WithParameterNames(parameterNames)
                .Find(instance);
        }

        public static RuntimeMember FindDirectInstance(object instance, IdentifierName memberName, int parameterCount) {
            return new MemberQuery(memberName, parameterCount, BindingFlags.Instance | BindingFlags.Static).FindMember(instance);
        }

        public static RuntimeMember FindInstance(object instance, IdentifierName memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length, BindingFlags.Instance | BindingFlags.Static)
                .WithParameterTypes(parameterTypes)
                .Find(instance);
        }

        public static RuntimeMember FindDirectInstance(object instance, IdentifierName memberName, Type[] parameterTypes) {
            return new MemberQuery(memberName, parameterTypes.Length, BindingFlags.Instance | BindingFlags.Static)
                .WithParameterTypes(parameterTypes)
                .FindMember(instance);
        }

        public TypedValue CreateInstance() {
            return new TypedValue(Type.Assembly.CreateInstance(Type.FullName), Type);
        }

        class MemberQuery {
            private readonly IdentifierName memberName;
            private readonly int parameterCount;
            private readonly BindingFlags flags;
            private IList<Type> parameterTypes;
            private IList<IdentifierName> parameterIdNames;

            public MemberQuery(IdentifierName memberName, int parameterCount, BindingFlags flags) {
                this.memberName = memberName;
                this.parameterCount = parameterCount;
                this.flags = flags;
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

            RuntimeMember FindMemberByName(object instance, Type targetType) {
                return FindMemberByName(instance, targetType, BindingFlags.Public)
                       ?? FindMemberByName(instance, targetType, BindingFlags.NonPublic);
            }

            RuntimeMember FindMemberByName(object instance, Type targetType, BindingFlags accessFlag) {
                foreach (MemberInfo memberInfo in targetType.GetMembers(flags | accessFlag | BindingFlags.FlattenHierarchy)) {
                    if (!MatchesName(memberInfo.Name)) continue;
                    RuntimeMember runtimeMember = MakeMember(memberInfo, instance);
                    if (Matches(runtimeMember)) return runtimeMember;
                }
                return null;
            }

            bool MatchesName(string name) {
                if (memberName.Matches(name)) return true;
                if (!memberName.MatchName.StartsWith("set") && !memberName.MatchName.StartsWith("get")) return false;
                return new IdentifierName(memberName.MatchName.Substring(3)).Matches(name);
            }

            static RuntimeMember MakeMember(MemberInfo memberInfo, object instance) {
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

            bool Matches(RuntimeMember runtimeMember) {
                if (!runtimeMember.MatchesParameterCount(parameterCount)) return false;
                if (parameterTypes != null) {
                    for (int i = 0; i < parameterCount; i++) {
                        if (runtimeMember.GetParameterType(i) != parameterTypes[i]) return false;
                    }
                }
                if (parameterIdNames != null) {
                    foreach (var name in parameterIdNames) {
                        if (!HasMatchingParameter(runtimeMember, name)) return false;
                    }
                }
                return true;
            }

            bool HasMatchingParameter(RuntimeMember runtimeMember, NameMatcher name) {
                for (int i = 0; i < parameterCount; i++) {
                  if (name.Matches(runtimeMember.GetParameterName(i))) return true;
                }
                return false;
            }

            RuntimeMember FindIndexerMember(object instance, Type targetType) {
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