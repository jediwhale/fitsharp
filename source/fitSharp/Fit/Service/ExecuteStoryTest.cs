// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service
{
    public delegate void WriteTestResult(Tree<Cell> tables, TestStatus status);

    public class ExecuteStoryTest {
        private readonly Processor<Cell> processor;
        private readonly WriteTestResult writer;
        private TestStatus testStatus;

        public ExecuteStoryTest(Processor<Cell> processor, WriteTestResult writer) {
            this.writer = writer;
            this.processor = processor;
        }

        public void DoTables(Tree<Cell> tables) {
            var elapsedTime = new ElapsedTime();
            Cell heading = tables.Branches[0].Branches[0].Branches[0].Value;
            Interpreter myFirstFixture;
            try {
                myFirstFixture = (Interpreter)processor.Parse(typeof(Interpreter), TypedValue.Void, tables.Branches[0]).Value;
            }
            catch (System.Exception e) {
                testStatus = new TestStatus();
                testStatus.MarkException(heading, e);
                writer(tables.Branches[0], testStatus);
                return;
            }
            testStatus = myFirstFixture.TestStatus;
 			testStatus.Summary["run date"] = DateTime.Now;
			testStatus.Summary["run elapsed time"] = elapsedTime;
            DoTables(myFirstFixture, tables);
        }

		private void DoTables(Interpreter firstFixture, Tree<Cell> theTables) {
            try {
                FlowInterpreter flowFixture = null;
                int tableCount = 1;
                foreach (Tree<Cell> table in theTables.Branches) {
                    try {
                        try {
                            Cell heading = table.Branches[0].Branches[0].Value;
                            if (heading != null && !testStatus.IsAbandoned) {
                                if (flowFixture == null) {
                                    var activeFixture = (Interpreter)processor.Parse(typeof(Interpreter), TypedValue.Void, table).Value;
                                    activeFixture.Prepare(firstFixture, table);
                                    if (activeFixture.IsInFlow(tableCount)) flowFixture = activeFixture as FlowInterpreter;
                                    if (!activeFixture.IsVisible) tableCount--;
                                    DoTable(table, activeFixture, flowFixture != null);
                                }
                                else {
                                    flowFixture.InterpretFlow(table);
                                }
                            }
                        }
                        catch (System.Exception e) {
                            testStatus.MarkException(table.Branches[0].Branches[0].Value, e);
                        }
                    }
                    catch (System.Exception e) {
                        if (!typeof(AbandonException).IsAssignableFrom(e.GetType())) throw;
                    }
                    tableCount++;
                }
                if (flowFixture != null) flowFixture.DoTearDown(theTables.Branches[0]);
            }
            catch (System.Exception e) {
                testStatus.MarkException(theTables.Branches[0].Branches[0].Branches[0].Value, e);
            }
			writer(theTables.Branches[0], testStatus);
		}

        public static void DoTable(Tree<Cell> table, Interpreter activeFixture, bool inFlow) {
            var activeFlowFixture = activeFixture as FlowInterpreter;
            if (activeFlowFixture != null) activeFlowFixture.DoSetUp(table);
            activeFixture.Interpret(table);
            if (activeFlowFixture != null && !inFlow) activeFlowFixture.DoTearDown(table);
        }
    }
}
