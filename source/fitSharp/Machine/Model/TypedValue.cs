// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Model {
    public struct TypedValue {
        public static readonly TypedValue Void = new TypedValue(null, typeof(void));

        public static TypedValue MakeInvalid(System.Exception exception) { return new TypedValue(exception, typeof(void)); }

        public object Value { get; private set; }
        public T GetValue<T>() { return (T)Value;} 
        public Type Type { get; private set; }

        public TypedValue(object value, Type type) : this() {
            Value = value;
            Type = type;
        }

        public TypedValue(object value): this(value, value != null ? value.GetType() : typeof(object)) {}

        public bool IsVoid { get { return Type == typeof (void) && Value == null; } }
        public bool IsValid { get { return Type != typeof (void) || Value == null; } }
        public bool IsNullOrEmpty { get { return Value == null || Type == typeof(DBNull) || Value.ToString().Length == 0; } }
        public string ValueString { get { return (IsVoid ? "void" : Value ?? "null").ToString(); } }

        public void ThrowExceptionIfNotValid() {
            if (!IsValid) throw (System.Exception) Value;
        }

        public override string ToString() { return ValueString; }
    }
}