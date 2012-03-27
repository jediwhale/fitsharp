// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class RunTestDefault: CellOperator, RunTestOperator {
        public bool CanRunTest(Tree<Cell> testTables, StoryTestWriter writer) {
            return true;
        }

        public TypedValue RunTest(Tree<Cell> testTables, StoryTestWriter writer) {
            new ExecuteStoryTest(Processor, writer).DoTables(testTables);
            return TypedValue.Void;
        }

        class ExecuteStoryTest {
            public ExecuteStoryTest(CellProcessor processor, StoryTestWriter writer) {
                this.writer = writer;
                this.processor = processor;
            }

            public void DoTables(Tree<Cell> tables) {
                processor.TestStatus = new TestStatus();
 			    processor.TestStatus.Summary["run date"] = DateTime.Now;
			    processor.TestStatus.Summary["run elapsed time"] = new ElapsedTime();
                var heading = tables.Branches[0].ValueAt(0, 0);
                try {
                    processor.Memory.Apply(i => i.As<SetUpTearDown>(s => s.SetUp()));
                    InterpretTables(tables);
                    processor.Memory.Apply(i => i.As<SetUpTearDown>(s => s.TearDown()));
                }
                catch (System.Exception e) {
                    processor.TestStatus.MarkException(heading, e);
                }
			    writer.WriteTest(tables, processor.TestStatus.Counts);
            }

            void InterpretTables(Tree<Cell> theTables) {
		        processor.TestStatus.TableCount = 1;
                flowFixture = new DefaultFlowInterpreter(null);
                foreach (var table in theTables.Branches) {
                    try {
                        try {
                            InterpretTable(table);
                        }
                        catch (System.Exception e) {
                            processor.TestStatus.MarkException(table.ValueAt(0, 0), e);
                        }
                    }
                    catch (System.Exception e) {
                        if (!typeof(AbandonException).IsAssignableFrom(e.GetType())) throw;
                    }

                    writer.WriteTable(table);

		            processor.TestStatus.TableCount++;
                }
                flowFixture.DoTearDown(theTables.Branches[0]);
		    }

            void InterpretTable(Tree<Cell> table) {
                var heading = table.ValueAt(0, 0);
                if (heading == null || processor.TestStatus.IsAbandoned) return;
                new InterpretFlow(processor, flowFixture)
                    .OnNewFixture(IsNewFlowFixture)
                    .DoTableFlow(table, 0);
            }

            bool IsNewFlowFixture(Interpreter fixture, int row) {
                var newFlowFixture = fixture as FlowInterpreter;
                if (row == 0 && newFlowFixture != null && newFlowFixture.IsInFlow(processor.TestStatus.TableCount)) {
                    flowFixture = newFlowFixture;
                    return true;
                }
                return false;
            }

            readonly CellProcessor processor;
            readonly StoryTestWriter writer;
            FlowInterpreter flowFixture;
        }
    }
}
