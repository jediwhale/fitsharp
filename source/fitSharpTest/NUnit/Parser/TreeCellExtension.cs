// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Model;

namespace fitSharp.Test.NUnit.Parser {
    public static class TreeCellExtension {
        public static string Format(this Tree<Cell> tree) {
            const string separator = " ";
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(tree.Value.GetAttribute(CellAttribute.Leader)))
                result.AppendFormat("{0}{1}", tree.Value.GetAttribute(CellAttribute.Leader), separator);
            result.Append(tree.Value.GetAttribute(CellAttribute.StartTag));
            foreach (var branch in tree.Branches)
                result.AppendFormat("{0}{1}", separator, branch.Format());
            if (!string.IsNullOrEmpty(tree.Value.GetAttribute(CellAttribute.Body)))
                result.AppendFormat("{0}{1}", separator, tree.Value.GetAttribute(CellAttribute.Body));
            result.Append(tree.Value.GetAttribute(CellAttribute.EndTag));
            if (!string.IsNullOrEmpty(tree.Value.GetAttribute(CellAttribute.Trailer)))
                result.AppendFormat("{0}{1}", separator, tree.Value.GetAttribute(CellAttribute.Trailer));
            return result.ToString();
        }
    }
}
