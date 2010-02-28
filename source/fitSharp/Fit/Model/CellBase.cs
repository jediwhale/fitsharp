// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Fit.Model {
    public class CellBase: Cell {
        CellAttributes Attributes { get; set; }
        readonly string text;

        public CellBase(string text) {
            this.text = text;
            Attributes = new CellAttributes();
        }

        public CellBase(CellBase other) {
            Attributes = new CellAttributes(other.Attributes);
            text = other.text;
        }

        public string Text { get { return text; } }

	    public void SetAttribute(CellAttribute key, string value) {
            Attributes.SetAttribute(key, value);
        }

	    public void AddToAttribute(CellAttribute key, string value, string format) {
	        Attributes.AddToAttribute(key, value, format);
	    }

	    public string GetAttribute(CellAttribute key) {
            return Attributes.GetAttribute(key);
        }

        public bool HasAttribute(CellAttribute key) {
            return Attributes.HasAttribute(key);
        }
    }
}
