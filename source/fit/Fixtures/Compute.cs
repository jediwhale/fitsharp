// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Fixtures {
    public class Compute: Fixture {
        Parse headerRow;

        public override void DoRows(Parse rows) {
            headerRow = rows;
            base.DoRows(rows.More);
        }

        public override void DoRow(Parse row) {
            string memberName = "!" + Processor.ParseTree<Cell, MemberName>(headerRow.Parts.Last);

            var parameterList = new List<Tree<Cell>>();
            Parse nameCell = headerRow.Parts;
            Parse valueCell = row.Parts;
            for (int i = 0;
                i < headerRow.Parts.Size - 1;
                i++, nameCell = nameCell.More, valueCell = valueCell.More) {
                parameterList.Add(new CellTreeLeaf(new GracefulName(nameCell.Text).ToString()));
                parameterList.Add(valueCell);
            }
            var result = Processor.Invoke(new TypedValue(SystemUnderTest), memberName,
                                          new EnumeratedTree<Cell>(parameterList));
            CellOperation.Check(result, row.Parts.Last);
        }
    }
}
