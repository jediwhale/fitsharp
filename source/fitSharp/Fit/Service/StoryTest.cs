// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class StoryTest {
        public StoryTest(CellProcessor processor, StoryTestWriter writer) {
            this.processor = processor;
            this.writer = writer;
        }

        public StoryTest WithInput(string withInput) {
            input = withInput;
            return this;
        }

        public StoryTest WithParsedInput(Tree<Cell> withParsedInput) {
            parsedInput = withParsedInput;
            isParsed = true;
            return this;
        }

        public bool IsExecutable {
            get { return ParsedInput != null && ParsedInput.Branches.Count > 0; }
        }

        public string Leader {
            get { return ParsedInput.Branches[0].Value.GetAttribute(CellAttribute.Leader); }
        }

        public void Execute() {
            Execute(processor);
        }

        public void Execute(CellProcessor cellProcessor) {
            cellProcessor.Operate<RunTestOperator>(ParsedInput, writer);
        }

        Tree<Cell> ParsedInput {
            get {
                if (!isParsed) {
	                parsedInput = processor.Compose(new StoryTestString(input));
                    isParsed = true;
                }
                return parsedInput;
            }
        }

        string input;
        Tree<Cell> parsedInput;
        private bool isParsed;

        readonly CellProcessor processor;
        readonly StoryTestWriter writer;
    }
}
