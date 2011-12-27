// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Fit.Fixtures;

namespace fit.Test.Acceptance {
    public class StoryTestFixture: DoFixture {

        public Parse TestResult(Parse theTest) {
            var writer = new StoryTestCopyWriter();
            var storyTest = new StoryTest(Processor, writer)
                .WithParsedInput(new Parse("div", string.Empty, theTest, null));
            storyTest.Execute(new Service.Service(Processor));
            return writer.ResultTables;
        }
    }
}
