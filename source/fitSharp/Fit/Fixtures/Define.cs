// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections;
using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class Define: Interpreter {
        public TestStatus TestStatus { get; private set; }

        private CellProcessor processor;

        public Define() { TestStatus = new TestStatus(); }

        public CellProcessor Processor {
            set { processor = value; }
        }

        public bool IsVisible { get { return false; } }

        public void Interpret(Tree<Cell> table) {
            var name = processor.Parse(typeof (MemberName), TypedValue.Void, new AlternatingTree(table.Branches[0])).GetValue<MemberName>();
            processor.Store(new Procedure(name.ToString(), table));
        }

        private class AlternatingTree: Tree<Cell> {
            private readonly Tree<Cell> baseTree;
            public AlternatingTree(Tree<Cell> baseTree) { this.baseTree = baseTree; }
            public Cell Value { get { return baseTree.Value; } }
            public bool IsLeaf { get { return baseTree.IsLeaf; } }
            public ReadList<Tree<Cell>> Branches { get { return new AlternatingList(baseTree.Branches); } }

            private class AlternatingList: ReadList<Tree<Cell>> {
                private readonly ReadList<Tree<Cell>> baseList;
                public AlternatingList(ReadList<Tree<Cell>> baseList) { this.baseList = baseList; }
                IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
                public Tree<Cell> this[int index] { get { return baseList[2 * index + 1]; } }
                public int Count { get { return (baseList.Count) / 2; } }

                public IEnumerator<Tree<Cell>> GetEnumerator() {
                    bool alternate = false;
                    foreach (Tree<Cell> branch in baseList) {
                        if (alternate) yield return branch;
                        alternate = !alternate;
                    }
                }
            }
        }
    }
}
