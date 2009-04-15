// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteError : ExecuteBase {
        private static readonly IdentifierName errorIdentifier = new IdentifierName("error");

        public override bool TryExecute(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result) {
            if (parameters.Verb != ExecuteParameters.Check || !errorIdentifier.Equals(parameters.Cell.Text)) return false;
            try {
                object actual = parameters.GetActual(processor);
                parameters.TestStatus.MarkWrong(parameters.Cell, actual.ToString());
            }
            catch {
                parameters.TestStatus.MarkRight(parameters.Cell);
            }
            return true;
        }
    }
}