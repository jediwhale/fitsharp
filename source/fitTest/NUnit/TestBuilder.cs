// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;

namespace fit.Test.NUnit {
    public class TestBuilder {
        readonly StringBuilder source = new StringBuilder();

        public TestBuilder() {}

        public TestBuilder(string source) {
            Append(source);
        }

        public TestBuilder Append(string fragment) {
            source.Append(fragment);
            return this;
        }

        public Parse Parse { get { return Parse.ParseRootFrom(source.ToString()); }}

        public StoryTest MakeStoryTest(CellProcessor processor) {
            return new StoryTest(processor,new StoryTestNullWriter()).WithParsedInput(Parse);
        }
    }
}
