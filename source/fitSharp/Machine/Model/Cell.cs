// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Text;

namespace fitSharp.Machine.Model {

    public interface Cell {
        string Text { get; }
        string Content { get; }
        IDictionary<CellAttribute, CellAttributeValue> Attributes { get; }
        TypedValue ParsedValue { get; set; }
    }

    public static class CellExtension {
        public static void SetAttribute(this Cell cell, CellAttribute key, string value) {
            if (!cell.HasAttribute(key)) cell.Attributes.Add(key, CellAttributeValue.Make(key));
            cell.Attributes[key].SetValue(value);
        }

        public static void ClearAttribute(this Cell cell, CellAttribute key) {
            cell.Attributes.Remove(key);
        }

        public static string GetAttribute(this Cell cell, CellAttribute key) {
            return cell.Attributes.ContainsKey(key) ? cell.Attributes[key].Value : string.Empty;
        }

        public static bool HasAttribute(this Cell cell, CellAttribute key) {
            return cell.Attributes.ContainsKey(key);
        }

        public static void FormatAttribute(this Cell cell, CellAttribute key, StringBuilder input) {
            if (cell.Attributes.ContainsKey(key)) cell.Attributes[key].Format(cell, input);
        }

        public static void SetTag(this Cell cell, string tag) {
            cell.SetAttribute(CellAttribute.StartTag, "<" + tag + ">");
            cell.SetAttribute(CellAttribute.EndTag, "</" + tag + ">");
        }
    }
}
