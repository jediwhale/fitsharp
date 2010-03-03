// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class CellSubstring: Cell {
        readonly Cell baseCell;

        public string Text { get; private set; }

        public CellSubstring(Cell baseCell, string text) {
            this.baseCell = baseCell;
            Text = text;
        }

        public string GetAttribute(CellAttribute key) { return baseCell.GetAttribute(key); }
        public void SetAttribute(CellAttribute key, string value) { baseCell.SetAttribute(key, value); }
        public void AddToAttribute(CellAttribute key, string value) { baseCell.AddToAttribute(key, value); }
    }
}
