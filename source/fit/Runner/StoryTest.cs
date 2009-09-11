// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Threading;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit {
    public interface StoryCommand {
        void Execute();
    }

    public class StoryTest: StoryCommand {
        private WriteTestResult writer;
        private Parse resultTables;

        public Parse Tables { get; private set; }

        public static Parse TestResult(Parse theTest) {
            var story = new StoryTest(theTest);
            story.writer = story.SaveTestResult;
            story.Execute();
            return story.resultTables;
        }

        public StoryTest() {
            writer = (t, s) => {};
        }

        public StoryTest(Parse theTables): this() {
            Tables = theTables;
        }

        public StoryTest(Parse theTables, WriteTestResult writer): this(theTables) {
            this.writer = writer;
        }

        public void Execute() {
		    var saveConfig = new Configuration(Context.Configuration);
            saveConfig.GetItem<Service.Service>().ApplicationUnderTest = saveConfig.GetItem<ApplicationUnderTest>();
            ExecuteOnConfiguration();
		    Context.Configuration = saveConfig;
        }

        public void ExecuteOnConfiguration() {
            string apartmentConfiguration = Context.Configuration.GetItem<Settings>().ApartmentState;
            if (apartmentConfiguration != null) {
                var desiredState = (ApartmentState)Enum.Parse(typeof(ApartmentState), apartmentConfiguration);
                if (Thread.CurrentThread.GetApartmentState() != desiredState) {
                    var thread = new Thread(DoTables);
                    thread.SetApartmentState(desiredState);
                    thread.Start();
                    thread.Join();
                    return;
                }
            }
            DoTables();
        }

        private void DoTables() {
            new ExecuteStoryTest(Context.Configuration.GetItem<Service.Service>(), writer).DoTables(new Parse("div", string.Empty, Tables, null));
        }

        private void SaveTestResult(Tree<Cell> theTables, TestStatus status) {
            var tables = (Parse) theTables.Value;
            for (Parse table = tables; table != null; table = table.More) {
                Parse newTable = table.Copy();
                if (resultTables == null) {
                    resultTables = newTable;
                }
                else {
                    resultTables.Last.More = newTable;
                }
            }
        }
    }

}
