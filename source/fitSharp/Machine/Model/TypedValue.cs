// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Reflection;
using System.Text;
using fitSharp.Machine.Exception;

namespace fitSharp.Machine.Model {
    public struct TypedValue {
        public static readonly TypedValue Void = new TypedValue(null, typeof(void));

        public static TypedValue MakeInvalid(System.Exception exception) { return new TypedValue(exception, typeof(void)); }

        public object Value { get; private set; }
        public T GetValue<T>() { return (T)Value;} 
        public T GetValueAs<T>() where T: class { return Value as T;}

        public R As<T, R>(Func<T, R> action, Func<R> notAction) where T: class {
            if (!HasValue) return notAction();
            var valueAs = GetValueAs<T>();
            return valueAs != null ? action(valueAs) : notAction();
        }

        public bool As<T>(Action<T> action, Action notAction) where T: class {
            return As<T, bool>(
                t => { action(t); return true; },
                () => { notAction(); return false; });
        }

        public void As<T>(Action<T> action) where T: class {
            As(action, () => { });
        }

        public void AsNot<T>(Action notAction) where T: class {
            As<T>(t => {}, notAction);
        }

        public Type Type { get; private set; }

        public TypedValue(object value, Type type) : this() {
            Value = value;
            Type = type;
        }

        public TypedValue(object value): this(value, value != null ? value.GetType() : typeof(object)) {}

        public bool IsVoid { get { return Type == typeof (void) && Value == null; } }
        public bool IsInvalid { get { return Type == typeof (void) && Value != null; } }
        public bool IsValid { get { return !IsInvalid; } }
        public bool IsObject { get { return IsValid && ! IsVoid; } }
        public bool HasValue { get { return IsObject && !IsNull; } }
        public bool HasValueAs<T>() { return HasValue && Value is T; }
        public bool IsNull { get { return IsObject && (Value == null || Type == typeof(DBNull)); } }
        public bool IsNullOrEmpty { get { return IsObject && (IsNull || Value.ToString().Length == 0); } }

        public string ValueString {
            get {
                if (HasValueAs<Array>()) {
                    var arrayString = new StringBuilder();
                    foreach (var value in (IEnumerable)Value) {
                        if (arrayString.Length > 0) arrayString.Append(", ");
                        arrayString.Append(value);
                    }
                    return arrayString.ToString();
                }
                return IsVoid ? "void" : (Value == null ? "null" : Value.ToString());
            }
        }

        public TypedValue ThrowExceptionIfNotValid() {
            if (IsValid) return this;
            var exception = GetValue<System.Exception>();
            throw exception is ValidationException? exception : new TargetInvocationException(exception);
        }

        public bool IsException<T>() {
            return !IsValid && Value is T;
        }

        public override string ToString() { return ValueString; }
    }
}
