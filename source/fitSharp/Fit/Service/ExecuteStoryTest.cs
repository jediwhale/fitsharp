// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public delegate void WriteTestResult(Tree<Cell> tables, TestCounts counts);

    public class ExecuteStoryTest {
        readonly CellProcessor processor;
        readonly WriteTestResult writer;

        Interpreter activeFixture;
        FlowInterpreter flowFixture;
        int tableCount;

        public ExecuteStoryTest(CellProcessor processor, WriteTestResult writer) {
            this.writer = writer;
            this.processor = processor;
        }

        public void DoTables(Tree<Cell> tables) {
            processor.TestStatus = new TestStatus();
 			processor.TestStatus.Summary["run date"] = DateTime.Now;
			processor.TestStatus.Summary["run elapsed time"] = new ElapsedTime();
            Cell heading = tables.Branches[0].Branches[0].Branches[0].Value;
            try {
                processor.Configuration.Apply(i => i.As<SetUpTearDown>(s => s.SetUp()));
                InterpretTables(tables);
                processor.Configuration.Apply(i => i.As<SetUpTearDown>(s => s.TearDown()));
            }
            catch (System.Exception e) {
                processor.TestStatus.MarkException(heading, e);
            }
			writer(tables, processor.TestStatus.Counts);
        }

        void GetStartingFixture(Tree<Cell> table) {
            try {
                activeFixture = processor.ParseTree<Cell, Interpreter>(table.Branches[0]);
                flowFixture = null;
            }
            catch (TypeMissingException) {
                activeFixture = processor.ParseTree<Cell, Interpreter>(new CellTree("fitlibrary.DoFixture"));
                flowFixture = (FlowInterpreter) activeFixture;
            }
        }

		void InterpretTables(Tree<Cell> theTables) {
            tableCount = 1;
            foreach (Tree<Cell> table in theTables.Branches) {
                try {
                    try {
                        InterpretTable(table);
                    }
                    catch (System.Exception e) {
                        processor.TestStatus.MarkException(table.Branches[0].Branches[0].Value, e);
                    }
                }
                catch (System.Exception e) {
                    if (!typeof(AbandonException).IsAssignableFrom(e.GetType())) throw;
                }
                activeFixture = null;
                tableCount++;
            }
            if (flowFixture != null) flowFixture.DoTearDown(theTables.Branches[0]);
		}

        void InterpretTable(Tree<Cell> table) {
            Cell heading = table.Branches[0].Branches[0].Value;
            if (heading == null || processor.TestStatus.IsAbandoned) return;
            if (flowFixture == null && tableCount == 1) GetStartingFixture(table);
            if (flowFixture == null) {
                if (activeFixture == null) activeFixture = processor.ParseTree<Cell, Interpreter>(table.Branches[0]);
                var activeFlowFixture = activeFixture as FlowInterpreter;
                if (activeFlowFixture != null && activeFlowFixture.IsInFlow(tableCount)) flowFixture = activeFlowFixture;
                if (!activeFixture.IsVisible) tableCount--;
                DoTable(table, activeFixture, flowFixture != null);
            }
            else {
                new InterpretFlow().DoTableFlow(processor, flowFixture, table);
            }
        }

        public static void DoTable(Tree<Cell> table, Interpreter activeFixture, bool inFlow) {
            var activeFlowFixture = activeFixture as FlowInterpreter;
            if (activeFlowFixture != null) activeFlowFixture.DoSetUp(table);
            activeFixture.Interpret(table);
            if (activeFlowFixture != null && !inFlow) activeFlowFixture.DoTearDown(table);
        }
    }
}
