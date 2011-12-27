// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
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

        //todo: flag argument - yuck
        public static void DoTable(Tree<Cell> table, Interpreter activeFixture, CellProcessor processor, bool inFlow) {
            var activeFlowFixture = activeFixture as FlowInterpreter;
            if (activeFlowFixture != null) activeFlowFixture.DoSetUp(processor, table);
            activeFixture.Interpret(processor, table);
            if (activeFlowFixture != null && !inFlow) activeFlowFixture.DoTearDown(table);
        }

        public static FlowInterpreter MakeDefaultFlowInterpreter(CellProcessor processor, TypedValue target) {
            var fixture = processor.Parse(typeof (Interpreter), target, defaultFlowTable.Branches[0]).GetValue<FlowInterpreter>();
            fixture.Interpret(processor, defaultFlowTable);
            return fixture;
        }

        static readonly CellTree defaultFlowTable = new CellTree(new CellTree("fitlibrary.DoFixture"));

        class ExecuteStoryTest {
            public ExecuteStoryTest(CellProcessor processor, StoryTestWriter writer) {
                this.writer = writer;
                this.processor = processor;
            }

            public void DoTables(Tree<Cell> tables) {
                processor.TestStatus = new TestStatus();
 			    processor.TestStatus.Summary["run date"] = DateTime.Now;
			    processor.TestStatus.Summary["run elapsed time"] = new ElapsedTime();
                Cell heading = tables.Branches[0].Branches[0].Branches[0].Value;
                try {
                    processor.Memory.Apply(i => i.As<SetUpTearDown>(s => s.SetUp()));
                    InterpretTables(tables);
                    processor.Memory.Apply(i => i.As<SetUpTearDown>(s => s.TearDown()));
                }
                catch (Exception e) {
                    processor.TestStatus.MarkException(heading, e);
                }
			    writer.WriteTest(tables, processor.TestStatus.Counts);
            }

            void InterpretTables(Tree<Cell> theTables) {
		        processor.TestStatus.TableCount = 1;
                foreach (Tree<Cell> table in theTables.Branches) {
                    try {
                        try {
                            InterpretTable(table);
                        }
                        catch (Exception e) {
                            processor.TestStatus.MarkException(table.Branches[0].Branches[0].Value, e);
                        }
                    }
                    catch (Exception e) {
                        if (!typeof(AbandonException).IsAssignableFrom(e.GetType())) throw;
                    }

                    writer.WriteTable(table);

                    activeFixture = null;
		            processor.TestStatus.TableCount++;
                }
                if (flowFixture != null) flowFixture.DoTearDown(theTables.Branches[0]);
		    }

            void InterpretTable(Tree<Cell> table) {
                Cell heading = table.Branches[0].Branches[0].Value;
                if (heading == null || processor.TestStatus.IsAbandoned) return;
                if (flowFixture == null && processor.TestStatus.TableCount == 1) GetStartingFixture(table);
                if (flowFixture == null) {
                    if (activeFixture == null) activeFixture = processor.ParseTree<Cell, Interpreter>(table.Branches[0]);
                    var activeFlowFixture = activeFixture as FlowInterpreter;
                    if (activeFlowFixture != null && activeFlowFixture.IsInFlow(processor.TestStatus.TableCount)) flowFixture = activeFlowFixture;
                    DoTable(table, activeFixture, processor, flowFixture != null);
                }
                else {
                    new InterpretFlow().DoTableFlow(processor, flowFixture, table);
                }
            }

            void GetStartingFixture(Tree<Cell> table) {
                try {
                    activeFixture = processor.ParseTree<Cell, Interpreter>(table.Branches[0]);
                    flowFixture = null;
                }
                catch (TypeMissingException) {
                    flowFixture = MakeDefaultFlowInterpreter(processor, TypedValue.Void);
                    activeFixture = flowFixture;
                }
            }

            readonly CellProcessor processor;
            readonly StoryTestWriter writer;

            Interpreter activeFixture;
            FlowInterpreter flowFixture;
        }
    }
}
