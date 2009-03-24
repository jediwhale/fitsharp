// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using System.Collections.Generic;
using fit;

namespace fitlibrary {

	public class SequenceFixture: FlowFixtureBase {

        public SequenceFixture() {}
        public SequenceFixture(object theSystemUnderTest): base(theSystemUnderTest) {}

        public override void DoRows(Parse theRows) {
            ProcessFlowRows(theRows);
        }

        protected override IEnumerable<Parse> MethodCells(CellRange theCells) {
            foreach (Parse cell in theCells.Cells) {
                yield return cell;
                break;
            }
        }

        protected override IEnumerable<Parse>  ParameterCells(CellRange theCells) {
            bool skip = true;
            foreach (Parse cell in theCells.Cells) {
                if (!skip) yield return cell;
                skip = false;
            }
        }
    }
}
