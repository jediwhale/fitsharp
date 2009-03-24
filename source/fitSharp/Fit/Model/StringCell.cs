// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class StringCell: Tree<Cell>, Cell {
        public StringCell(string text) { Text = text; }
        public string Body { get { return Text; } }
        public void SetBody(string body) { Text = body; }
        public string Text { get; private set; }
        public override Cell Value { get { return this; } }
        public override bool IsLeaf { get { return true; } }
        public override ReadList<Tree<Cell>> Branches { get { throw new InvalidOperationException(); } }
    }
}