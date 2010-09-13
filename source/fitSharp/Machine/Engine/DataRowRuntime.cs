using System;
using System.Data;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class DataRowRuntime<T,P>: Operator<T, P>, RuntimeOperator<T> where P: class, Processor<T> {
        public bool CanCreate(string memberName, Tree<T> parameters) { return false; }
        public TypedValue Create(string memberName, Tree<T> parameters) { return TypedValue.Void; }

        public bool CanInvoke(TypedValue instance, string memberName, Tree<T> parameters) {
            int parameterCount = parameters.Branches.Count;
            return instance.Type == typeof (DataRow) && (parameterCount == 0);
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters) {
            var dataRow = (DataRow) instance.Value;
            object value = dataRow[memberName];
            if (value is DBNull) value = null;
            return new TypedValue(value, dataRow.Table.Columns[memberName].DataType);
        }
    }
}
