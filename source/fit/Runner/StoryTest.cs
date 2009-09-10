// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Threading;
using fitSharp.Fit.Model;
using fit.exception;
using fitlibrary;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

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
            new ExecuteStoryTest(writer).DoTables(Tables);
        }

        private void SaveTestResult(Parse theTables, TestStatus status) {
            for (Parse table = theTables; table != null; table = table.More) {
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

    public delegate void WriteTestResult(Parse tables, TestStatus status);

    public class ExecuteStoryTest {
        private readonly WriteTestResult writer;
        private readonly TestStatus testStatus;

        public ExecuteStoryTest(WriteTestResult writer) {
            this.writer = writer;
            testStatus = new TestStatus();
        }

        public TestStatus DoTables(Parse tables) {
 			testStatus.Summary["run date"] = DateTime.Now;
			testStatus.Summary["run elapsed time"] = new ElapsedTime();
            Parse heading = tables.At(0, 0, 0);
            Fixture myFirstFixture;
            try {
                object instance = Fixture.LoadClass(tables.At(0, 0, 0).Text);
                myFirstFixture = instance as Fixture ?? new DoFixture(instance);
            }
            catch (Exception e) {
                testStatus.MarkException(heading, e);
                writer(tables, testStatus);
                return testStatus;
            }
            myFirstFixture.TestStatus = testStatus;
            myFirstFixture.Processor = Context.Configuration.GetItem<Service.Service>();
            DoTables(myFirstFixture, tables);
            return testStatus;
        }

		private void DoTables(Fixture firstFixture, Parse theTables) {
            try {
                FlowFixtureBase flowFixture = null;
                int tableCount = 1;
                for (Parse table = theTables; table != null; table = table.More) {
                    try {
                        try {
                            Parse heading = table.At(0, 0, 0);
                            if (heading != null && !testStatus.IsAbandoned) {
                                if (flowFixture == null) {
                                    object instance = Fixture.LoadClass(heading.Text);
                                    var activeFixture = instance as Fixture ?? new DoFixture(instance);
                                    activeFixture.Prepare(firstFixture, table);
                                    if (activeFixture.IsInFlow(tableCount)) flowFixture = activeFixture as FlowFixtureBase;
                                    if (!activeFixture.IsVisible) tableCount--;
                                    DoTable(table, activeFixture, flowFixture != null);
                                }
                                else {
                                    flowFixture.DoFlowTable(table);
                                }
                            }
                        }
                        catch (Exception e) {
                            testStatus.MarkException(table.At(0, 0, 0), e);
                        }
                    }
                    catch (AbandonStoryTestException) {}
                    tableCount++;
                }
                if (flowFixture != null) flowFixture.DoTearDown(theTables);
            }
            catch (Exception e) {
                testStatus.MarkException(theTables.Parts.Parts, e);
            }
			writer(theTables, testStatus);
		}

        public static void DoTable(Parse table, Fixture activeFixture, bool inFlow) {
            var activeFlowFixture = activeFixture as FlowFixtureBase;
            if (activeFlowFixture != null) activeFlowFixture.DoSetUp(table);
            activeFixture.DoTable(table);
            if (activeFlowFixture != null && !inFlow) activeFlowFixture.DoTearDown(table);
        }
    }
}
