// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class RuntimeProcedure: CellOperator, RuntimeOperator<Cell>
    {
        public bool CanCreate(string memberName, Tree<Cell> parameters) { return false; }

        public TypedValue Create(string memberName, Tree<Cell> parameters) { return TypedValue.Void; }

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return Processor.Contains(new Procedure(memberName));
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var procedure = Processor.Load(new Procedure(memberName));
            return procedure.Invoke(Processor, instance);
        }
    }
}
