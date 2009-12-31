// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Threading;
using fitSharp.Fit.Service;
using fitSharp.Machine.Application;

namespace fit {
    public interface StoryCommand {
        void Execute(Configuration configuration);
    }

    public class StoryTest: StoryCommand {
        private readonly WriteTestResult writer;

        public Parse Tables { get; private set; }

        public StoryTest() {
            writer = (t, s) => {};
        }

        public StoryTest(Parse theTables): this() {
            Tables = theTables;
        }

        public StoryTest(Parse theTables, WriteTestResult writer): this(theTables) {
            this.writer = writer;
        }

        public void Execute(Configuration configuration) {
		    var newConfig = new Configuration(configuration);
            ExecuteOnConfiguration(newConfig);
        }

        public void ExecuteOnConfiguration(Configuration configuration) {
            string apartmentConfiguration = configuration.GetItem<Settings>().ApartmentState;
            if (apartmentConfiguration != null) {
                var desiredState = (ApartmentState)Enum.Parse(typeof(ApartmentState), apartmentConfiguration);
                if (Thread.CurrentThread.GetApartmentState() != desiredState) {
                    var thread = new Thread(o => DoTables((Configuration)o));
                    thread.SetApartmentState(desiredState);
                    thread.Start(configuration);
                    thread.Join();
                    return;
                }
            }
            DoTables(configuration);
        }

        private void DoTables(Configuration configuration) {
            new ExecuteStoryTest(new Service.Service(configuration), writer)
                .DoTables(new Parse("div", string.Empty, Tables, null));
        }
    }
}
