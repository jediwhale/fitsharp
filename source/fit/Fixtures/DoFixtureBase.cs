// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit;
using fit.Model;

namespace fitlibrary {

	public class DoFixtureBase: FlowFixtureBase {

        public DoFixtureBase() {}
        public DoFixtureBase(object theSystemUnderTest): base(theSystemUnderTest) {}

        protected override IEnumerable<Parse> MethodCells(CellRange theCells) {
            bool alternate = true;
            foreach (Parse cell in theCells.Cells) {
                if (alternate) yield return cell;
                alternate = !alternate;
            }
        }

        protected override IEnumerable<Parse> ParameterCells(CellRange theCells) {
            bool alternate = false;
            foreach (Parse cell in theCells.Cells) {
                if (alternate) yield return cell;
                alternate = !alternate;
            }
        }
    }
}
