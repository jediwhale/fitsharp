// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;

namespace fit {
    public interface StoryCommand {
        void Execute(Configuration configuration);
    }

    public class StoryTest: StoryCommand {
        private readonly StoryTestWriter writer;

        public Parse Tables { get; private set; }

        public StoryTest() {
            writer = new StoryTestNullWriter();
        }

        public StoryTest(Parse theTables): this() {
            Tables = theTables;
        }

        public StoryTest(Parse theTables, StoryTestWriter writer): this(theTables) {
            this.writer = writer;
        }

        public void Execute(Configuration configuration) {
		    var newConfig = configuration.Copy();
            ExecuteOnConfiguration(newConfig);
        }

        public void ExecuteOnConfiguration(Configuration configuration) {
            new ExecuteStoryTest(new Service.Service(configuration), writer)
                .DoTables(Tables);
        }
    }
}
