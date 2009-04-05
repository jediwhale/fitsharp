// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Web;
using fitSharp.Fit.Model;

namespace fit.Model {
    public class CellSubstring: Cell {
        private readonly Parse baseCell;
        private readonly int bodyStart;
        private readonly int textStart;

        public CellSubstring(Parse baseCell, string prefix) {
            this.baseCell = baseCell;
            textStart = prefix.Length;
            bodyStart = baseCell.Body.StartsWith(prefix) ? prefix.Length : HttpUtility.HtmlEncode(prefix).Length;
        }

        public string Body { get { return baseCell.Body.Substring(bodyStart); } }
        public string Text { get { return baseCell.Text.Substring(textStart); } }
        public string GetAttribute(string key) { return baseCell.GetAttribute(key); }
        public void SetAttribute(string key, string value) { baseCell.SetAttribute(key, value); }
        public void AddToAttribute(string key, string value, string format) { baseCell.AddToAttribute(key, value, format); }

        public Parse ParseCell { get { return baseCell; } }
    }
}
