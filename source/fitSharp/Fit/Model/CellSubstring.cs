// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class CellSubstring: Cell {
        readonly Cell baseCell;

        public string Text { get; private set; }
        public string Content { get { return Text; } } // TODO(kimgr): is this right, or should we trim here too?

        public IDictionary<CellAttribute, CellAttributeValue> Attributes { get { return baseCell.Attributes; } }

        public TypedValue ParsedValue { get; set; }

        public CellSubstring(Cell baseCell, string text) {
            this.baseCell = baseCell;
            Text = text;
        }
    }
}
