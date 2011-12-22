// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Data;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class InvokeDataRow<T,P>: Operator<T, P>, InvokeOperator<T> where P: class, Processor<T> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
            var parameterCount = parameters.Branches.Count;
            return instance.Type == typeof (DataRow) && (parameterCount == 0);
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
            var dataRow = (DataRow) instance.Value;
            object value = dataRow[memberName.Name];
            if (value is DBNull) value = null;
            return new TypedValue(value, dataRow.Table.Columns[memberName.Name].DataType);
        }
    }
}
