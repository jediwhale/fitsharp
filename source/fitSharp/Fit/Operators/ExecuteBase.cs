// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public abstract class ExecuteBase: ExecuteOperator<Cell> {

        public abstract bool IsMatch(Processor<Cell> processor, ExecuteParameters parameters);
        public abstract TypedValue Execute(Processor<Cell> processor, ExecuteParameters parameters);

        public bool CanExecute(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters) {
            return IsMatch(processor, new ExecuteParameters((ExecuteContext) instance.Value, parameters));
        }

        public TypedValue Execute(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters) {
            return Execute(processor, new ExecuteParameters((ExecuteContext)instance.Value, parameters));
        }
    }
}