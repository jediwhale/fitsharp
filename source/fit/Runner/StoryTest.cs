// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Threading;
using fit.Engine;
using fit.exception;
using fitlibrary;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit {
    public interface StoryCommand {
        void Execute();
        Counts Counts { get; }
    }

    public class StoryTest: StoryCommand {
        public TestStatus TestStatus { get; private set; }

        public FixtureListener Listener { get; private set; }
        public Parse Tables { get; private set; }

        public static Parse TestResult(Parse theTest) {
            var listener = new SpecifyListener();
            var story = new StoryTest(theTest, listener);
            story.Execute();
            return listener.Tables;
        }

        public StoryTest() {
            Listener = new NullFixtureListener();
            TestStatus = new TestStatus();
        }

        public StoryTest(Parse theTables): this() {
            Tables = theTables;
        }

        public StoryTest(Parse theTables, FixtureListener theListener): this(theTables) {
            Listener = theListener;
        }

        public Counts Counts { get { return TestStatus.Counts; } }

        public void Execute() {
		    var saveConfig = new Configuration(Context.Configuration);
            saveConfig.GetItem<Service>().ApplicationUnderTest = saveConfig.GetItem<ApplicationUnderTest>();
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
 			TestStatus.Summary["run date"] = DateTime.Now;
			TestStatus.Summary["run elapsed time"] = new RunTime();
            Parse heading = Tables.At(0, 0, 0);
            try {
                object instance = Fixture.LoadClass(Tables.At(0, 0, 0).Text);
                myFirstFixture = instance as Fixture;
                if (myFirstFixture == null) {
                    myFirstFixture = new DoFixture(instance);
                }
            }
            catch (Exception e) {
                myFirstFixture = new Fixture();
                myFirstFixture.TestStatus = TestStatus;
                myFirstFixture.Exception(heading, e);
                Listener.TableFinished(Tables);
                Listener.TablesFinished(Tables, Counts);
                return;
            }
            myFirstFixture.TestStatus = TestStatus;
            myFirstFixture.Service = Context.Configuration.GetItem<Service>();
            DoTables(myFirstFixture, Tables);
        }

		private void DoTables(Fixture firstFixture, Parse theTables) {
            try {
                FlowFixtureBase flowFixture = null;
                int tableCount = 1;
                for (Parse table = theTables; table != null; table = table.More) {
                    try {
                        try {
                            Parse heading = table.At(0, 0, 0);
                            if (heading != null && !TestStatus.IsAbandoned) {
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
                            firstFixture.Exception(table.At(0, 0, 0), e);
                        }
                    }
                    catch (AbandonStoryTestException) {}
                    Listener.TableFinished(table);
                    tableCount++;
                }
                if (flowFixture != null) flowFixture.DoTearDown(theTables);
            }
            catch (Exception e) {
                firstFixture.Exception(theTables.Parts.Parts, e);
                Listener.TableFinished(theTables);
            }
			Listener.TablesFinished(theTables, Counts);
		}

        public static void DoTable(Parse table, Fixture activeFixture, bool inFlow) {
            var activeFlowFixture = activeFixture as FlowFixtureBase;
            if (activeFlowFixture != null) activeFlowFixture.DoSetUp(table);
            activeFixture.DoTable(table);
            if (activeFlowFixture != null && !inFlow) activeFlowFixture.DoTearDown(table);
        }

        private Fixture myFirstFixture;

        private class SpecifyListener: FixtureListener {

            public void TablesFinished(Parse theTables, Counts counts) {}

            public void TableFinished(Parse finishedTable) {
                Parse newTable = ParseNode.Clone(finishedTable);
                if (myTables == null) {
                    myTables = newTable;
                }
                else {
                    myTables.Last.More = newTable;
                }
            }

            public Parse Tables {get {return myTables;}}

            private Parse myTables;

        }
    }
}
