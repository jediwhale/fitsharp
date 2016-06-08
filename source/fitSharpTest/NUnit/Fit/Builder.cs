// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Test.NUnit.Fit {
    public class Builder {
        public static CellProcessorBase CellProcessor() {
            return new CellProcessorBase(new TypeDictionary(), new CellOperators());
        }

        public static Tree<Cell> ParseHtmlTable(string input) {
            return new HtmlTables(text => new TreeList<Cell>(new CellBase(text))).Parse(input).Branches[0];
        } 
    }
}
