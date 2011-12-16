// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class Include: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            var keywords = new IncludeKeywords(processor);
            processor.InvokeWithThrow(
                new TypedValue(keywords),
                table.Branches[0].Branches[1].Value.Text,
                new CellTree(table.Branches[0].Branches[2]));
            table.Branches[0].Branches[0].Value.SetAttribute(CellAttribute.Folded, keywords.Result);
        }

        class IncludeKeywords {
            public IncludeKeywords(CellProcessor processor) {
                this.processor = processor;
            }

            public string Result { get; private set; }

            public void Text(string storyTestText) {
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
