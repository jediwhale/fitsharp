// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public abstract class ExecuteBase: ExecuteOperator<Cell> {

        public abstract bool TryExecute(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result);

        public bool TryExecute(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            return TryExecute(processor, new ExecuteParameters((ExecuteContext)instance.Value, parameters), ref result);
        }
    }
}
