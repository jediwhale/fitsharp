// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public abstract class ExecuteBase: Operator<Cell>, ExecuteOperator<Cell> {

        public abstract bool CanExecute(ExecuteParameters parameters);
        public abstract TypedValue Execute(ExecuteParameters parameters);

        public bool CanExecute(TypedValue instance, Tree<Cell> parameters) {
            return CanExecute(new ExecuteParameters((ExecuteContext) instance.Value, parameters));
        }

        public TypedValue Execute(TypedValue instance, Tree<Cell> parameters) {
            return Execute(new ExecuteParameters((ExecuteContext)instance.Value, parameters));
        }
    }
}