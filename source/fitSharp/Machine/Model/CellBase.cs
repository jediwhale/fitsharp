// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class CellBase: Cell {
        readonly Dictionary<CellAttribute, CellAttributeValue> attributes = new Dictionary<CellAttribute, CellAttributeValue>();

        public string Text { get; private set; }
        public string Content { get { return Text.Trim(); } }

        public IDictionary<CellAttribute, CellAttributeValue> Attributes { get { return attributes; } }

        public TypedValue ParsedValue { get; set; }

        public CellBase(string text) {
            Text = text;
        }

        public CellBase(string text, string tag): this(text) {
            this.SetAttribute(CellAttribute.StartTag, "<" + tag + ">");
            this.SetAttribute(CellAttribute.EndTag, "</" + tag + ">");
        }

        public CellBase(CellBase other) {
            attributes = new Dictionary<CellAttribute, CellAttributeValue>(other.attributes);
            Text = other.Text;
        }
    }
}
