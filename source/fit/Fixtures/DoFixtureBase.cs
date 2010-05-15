// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using System.Linq;
using fit;
using fit.Model;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class DoFixtureBase: FlowFixtureBase {

        public DoFixtureBase() {}
        public DoFixtureBase(object theSystemUnderTest): base(theSystemUnderTest) {}

        protected override IEnumerable<Parse> MethodCells(CellRange theCells) {
            return theCells.Cells.Alternate();
        }

        protected override IEnumerable<Parse> ParameterCells(CellRange theCells) {
            return theCells.Cells.Skip(1).Alternate();
        }
    }
}
