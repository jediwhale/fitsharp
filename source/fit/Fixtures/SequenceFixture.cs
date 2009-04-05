// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit;
using fit.Model;

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
