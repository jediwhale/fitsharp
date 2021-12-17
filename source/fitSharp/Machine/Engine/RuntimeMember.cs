// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public interface RuntimeMember {
        TypedValue Invoke(object[] parameters);
        bool MatchesParameterCount(int count);
        Type GetParameterType(int index);
        string GetParameterName(int index);
        Type ReturnType { get; }
        string Name { get; }
    }

    public abstract class ReflectionMember: RuntimeMember {
        protected readonly object instance;
        protected readonly MemberInfo info;

        protected ReflectionMember(MemberInfo info, object instance) {
            this.instance = instance;
            this.info = info;
        } 

        public string Name => (info.DeclaringType == null ? "" : info.DeclaringType.FullName + ":") + info.Name;
        public virtual string GetParameterName(int index) { return info.Name; }

        public TypedValue Invoke(object[] parameters) {
            try {
                return TryInvoke(parameters);
            } catch (TargetInvocationException e) {
                if (e.InnerException is ValidationException) throw e.InnerException;
                throw;
            }
        }

        protected abstract TypedValue TryInvoke(object[] parameters);
        public abstract bool MatchesParameterCount(int count);
        public abstract Type GetParameterType(int index);
        public abstract Type ReturnType { get; }
    }

    class MethodMember: ReflectionMember {
        public MethodMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        protected MethodInfo Info => (MethodInfo) info;

        public override bool MatchesParameterCount(int count) { return Info.GetParameters().Length == count; }

        public override Type GetParameterType(int index) {
            return Info.GetParameters()[index].ParameterType;
        }

        public override string GetParameterName(int index) {
            return Info.GetParameters()[index].Name;
        }

        protected override TypedValue TryInvoke(object[] parameters) {
            object result;
            if (Info.IsGenericMethod) {
                result = Info.Invoke(instance, parameters);
            }
            else {
                //todo: hack to  pick different overloaded member if parameter type (via parse symbol) is different
                //todo: breaks generic method execution
                var type = info.DeclaringType;
                result = type.InvokeMember(info.Name,
                                               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                               | BindingFlags.InvokeMethod | BindingFlags.Static,
                                               null, instance, parameters);
            }
            return new TypedValue(result, Info.ReturnType != typeof(void) && result != null ? result.GetType() : Info.ReturnType); //todo: push this into TypedValue
        }

        public override Type ReturnType => Info.ReturnType;
    }

    class ExtensionMember : MethodMember {
        public ExtensionMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        protected override TypedValue TryInvoke(object[] parameters) {
            var extensionParms = new object[parameters.Length + 1];
            extensionParms[0] = instance;
            for (var i = 1; i < extensionParms.Length; i++) extensionParms[i] = parameters[i - 1];
            var result = Info.Invoke(null, extensionParms);
            return new TypedValue(result, Info.ReturnType != typeof(void) && result != null ? result.GetType() : Info.ReturnType); //todo: push this into TypedValue
        }
        
        public override Type GetParameterType(int index) {
            return Info.GetParameters()[index + 1].ParameterType;
        }
    }

    class IndexerMember: MethodMember {
        readonly string key;

        public IndexerMember(MemberInfo memberInfo, object instance, string key): base(memberInfo, instance) {
            this.key = key;
        }

        protected override TypedValue TryInvoke(object[] parameters) {
            return base.TryInvoke(new object[] {key});
        }
    }

    class FieldMember: ReflectionMember {
        public FieldMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        FieldInfo Info => (FieldInfo) info;

        public override Type GetParameterType(int index) {
            return Info.FieldType;
        }

        public override bool MatchesParameterCount(int count) { return count == 0 || count == 1; }

        protected override TypedValue TryInvoke(object[] parameters) {
            var type = info.DeclaringType;
            var result = type.InvokeMember(info.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                              | (parameters.Length == 0 ? BindingFlags.GetField : BindingFlags.SetField)
                                              | BindingFlags.Static,
                                              null, instance, parameters);

            return new TypedValue(result, parameters.Length == 0 ? Info.FieldType : typeof(void));
        }

        public override Type ReturnType => Info.FieldType;
    }

    class PropertyMember: ReflectionMember {
        public PropertyMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        PropertyInfo Info => (PropertyInfo) info;

        public override Type GetParameterType(int index) {
            return Info.PropertyType;
        }

        public override bool MatchesParameterCount(int count) { return count == 0 && Info.CanRead || count == 1 && Info.CanWrite; }

        protected override TypedValue TryInvoke(object[] parameters) {
            var type = info.DeclaringType;
            var result = type.InvokeMember(info.Name,
                                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                              | (parameters.Length == 0 ? BindingFlags.GetProperty : BindingFlags.SetProperty)
                                              | BindingFlags.Static,
                                              null, instance, parameters);

            return new TypedValue(result, parameters.Length == 0 ? Info.PropertyType : typeof(void));
        }

        public override Type ReturnType => Info.PropertyType;
    }

    class ConstructorMember: ReflectionMember {
        public ConstructorMember(MemberInfo memberInfo, object instance): base(memberInfo, instance) {}

        ConstructorInfo Info => (ConstructorInfo) info;

        public override Type GetParameterType(int index) {
            return Info.GetParameters()[index].ParameterType;
        }

        public override string GetParameterName(int index) {
            return Info.GetParameters()[index].Name;
        }

        public override bool MatchesParameterCount(int count) { return Info.GetParameters().Length == count; }

        protected override TypedValue TryInvoke(object[] parameters) {
            var type = info.DeclaringType;
            var result = Info.Invoke(parameters);
            return new TypedValue(result, type);
        }

        public override Type ReturnType => info.DeclaringType;
    }
}
