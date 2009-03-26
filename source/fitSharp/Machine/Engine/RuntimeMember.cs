// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public abstract class RuntimeMember {
        protected object instance;
        protected readonly MemberInfo info;

        protected RuntimeMember(MemberInfo info, object instance) {
            this.instance = instance;
            this.info = info;
        } 

        public string Name { get { return info.Name; }}

        public abstract TypedValue Invoke(object[] parameters);
        public abstract bool MatchesParameterCount(int count);
        public abstract Type GetParameterType(int index);
        public abstract Type ReturnType { get; }
    }

    class MethodMember: RuntimeMember {
        public MethodMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        private MethodInfo Info { get { return (MethodInfo) info; } }

        public override bool MatchesParameterCount(int count) { return Info.GetParameters().Length == count; }

        public override Type GetParameterType(int index) {
            return Info.GetParameters()[index].ParameterType;
        }

        public override TypedValue Invoke(object[] parameters) {
            Type type = info.DeclaringType;
            object result = type.InvokeMember(info.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                              | BindingFlags.InvokeMethod | BindingFlags.Static,
                                              null, instance, parameters);

            return new TypedValue(result, Info.ReturnType);
        }

        public override Type ReturnType { get { return Info.ReturnType; } }
    }

    class IndexerMember: MethodMember {
        private readonly string key;

        public IndexerMember(MemberInfo memberInfo, object instance, string key): base(memberInfo, instance) {
            this.key = key;
        }

        public override TypedValue Invoke(object[] parameters) {
            return base.Invoke(new object[] {key});
        }
    }

    class FieldMember: RuntimeMember {
        public FieldMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        private FieldInfo Info { get { return (FieldInfo) info; } }

        public override Type GetParameterType(int index) {
            return Info.FieldType;
        }

        public override bool MatchesParameterCount(int count) { return count == 0 || count == 1; }

        public override TypedValue Invoke(object[] parameters) {
            Type type = info.DeclaringType;
            object result = type.InvokeMember(info.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                              | (parameters.Length == 0 ? BindingFlags.GetField : BindingFlags.SetField)
                                              | BindingFlags.Static,
                                              null, instance, parameters);

            return new TypedValue(result, parameters.Length == 0 ? Info.FieldType : typeof(void));
        }

        public override Type ReturnType { get { return Info.FieldType; } }
    }

    class PropertyMember: RuntimeMember {
        public PropertyMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        private PropertyInfo Info { get { return (PropertyInfo) info; } }

        public override Type GetParameterType(int index) {
            return Info.PropertyType;
        }

        public override bool MatchesParameterCount(int count) { return count == 0 && Info.CanRead || count == 1 && Info.CanWrite; }

        public override TypedValue Invoke(object[] parameters) {
            Type type = info.DeclaringType;
            object result = type.InvokeMember(info.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                              | (parameters.Length == 0 ? BindingFlags.GetProperty : BindingFlags.SetProperty)
                                              | BindingFlags.Static,
                                              null, instance, parameters);

            return new TypedValue(result, parameters.Length == 0 ? Info.PropertyType : typeof(void));
        }

        public override Type ReturnType { get { return Info.PropertyType; } }
    }

    class ConstructorMember: RuntimeMember {
        public ConstructorMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        private ConstructorInfo Info { get { return (ConstructorInfo) info; } }

        public override Type GetParameterType(int index) {
            return Info.GetParameters()[index].ParameterType;
        }

        public override bool MatchesParameterCount(int count) { return Info.GetParameters().Length == count; }

        public override TypedValue Invoke(object[] parameters) {
            Type type = info.DeclaringType;
            object result = type.InvokeMember(info.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                              | BindingFlags.CreateInstance,
                                              null, null, parameters);
            return new TypedValue(result, type);
        }

        public override Type ReturnType { get { return info.DeclaringType; } }
    }
}