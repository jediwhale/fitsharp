// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using System.Collections.Generic;
using fit;

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
