// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fit.Operators;
using fitlibrary;
using fitlibrary.table;

namespace fit.Test.Acceptance {
    public class CellMaking: DoFixture {

        public string MakeString(string theValue) {
            return Encode(CellFactoryRepository.Instance.Make(theValue));
        }

        public string MakeTable(Table theValue) {
            return Encode(CellFactoryRepository.Instance.Make(theValue));
        }

        private string Encode(Parse theCell) {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("tag: {0}", theCell.Tag);
            if (theCell.Body != null && theCell.Body.Length > 0) result.AppendFormat(" body: {0}", theCell.Body);
            if (theCell.Parts != null) result.AppendFormat(" [ {0}]", Encode(theCell.Parts));
            if (theCell.More != null) result.AppendFormat(", {0}", Encode(theCell.More));
            return result.ToString();
        }

    }
}