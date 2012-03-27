// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class TypeNameFixture: ColumnFixture {

        public string GracefulName;

        public IdentifierName IdentifierName { get { return new GracefulName(GracefulName).IdentifierName; } }

        public string TypeName;

        public string FullName {
            get {
                var interpreter = Processor.ParseTree<Cell, Interpreter>(new CellTree(TypeName));
                var adapter = interpreter as DomainAdapter;
                return adapter != null && adapter.SystemUnderTest != null
                    ? adapter.SystemUnderTest.GetType().FullName
                    : interpreter.GetType().FullName;
            }
        }
    }
}
