// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class Include: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            var keywords = new IncludeKeywords(processor);
            var currentRow = new EnumeratedTree<Cell>(table.Branches[0].Branches.Skip(1));
            var selector = new DoRowSelector();
            processor.ExecuteWithThrow(keywords, selector.SelectMethodCells(currentRow),
                                                                    selector.SelectParameterCells(currentRow),
                                                                    currentRow.Branches[0].Value);
            table.Branches[0].Branches[0].Value.SetAttribute(CellAttribute.Folded, keywords.Result);
        }

        class IncludeKeywords {
            public IncludeKeywords(CellProcessor processor) {
                this.processor = processor;
            }

            public string Result { get; private set; }

            public void Text<T>(T storyTestSource) {
                String(storyTestSource.ToString());
            }

            public void String(string storyTestText) {
                var writer = new StoryTestStringWriter(processor);
                var storyTest = new StoryTest(processor, writer).WithInput(storyTestText);
                if (storyTest.IsExecutable) {
                    storyTest.Execute();
                    Result = writer.Tables;
                }
                else {
                    Result = storyTestText;
                }
            }

            readonly CellProcessor processor;
        }
    }
}
