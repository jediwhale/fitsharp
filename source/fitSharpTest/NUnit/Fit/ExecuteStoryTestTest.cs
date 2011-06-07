// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ExecuteStoryTestTest {
        CellProcessorBase processor;
        CellTree tables;
        ExecuteStoryTest execute;

        [SetUp] public void SetUp() {
            processor = new CellProcessorBase();
            processor.AddOperator(new TestParseInterpreter());
            execute = new ExecuteStoryTest(processor, (t, c) => {});
            tables = new CellTree(new CellTree(new CellTree("myfixture")));
        }
    
        [Test] public void CreatesFixtureOnce() {
            SampleFixture.Count = 0;
            execute.DoTables(tables);
            Assert.AreEqual(1, SampleFixture.Count);
        }

        [Test] public void SetsUpConfiguration() {
            processor.Configuration.SetItem(typeof(SampleItem), new SampleItem());
            execute.DoTables(tables);
            Assert.IsTrue(processor.Configuration.GetItem<SampleItem>().IsSetUp);
        }

        [Test] public void TearsDownConfiguration() {
            processor.Configuration.SetItem(typeof(SampleItem), new SampleItem());
            execute.DoTables(tables);
            Assert.IsTrue(processor.Configuration.GetItem<SampleItem>().IsTearDown);
        }

        private class TestParseInterpreter: CellOperator, ParseOperator<Cell> {
            public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
                return typeof (Interpreter).IsAssignableFrom(type);
            }

            public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
                return new TypedValue(new SampleFixture());
            }
        }

        private class SampleFixture: Interpreter {
            public static int Count;

            public SampleFixture() { Count++; }

            public bool IsVisible { get { return true; } }
            public void Interpret(Tree<Cell> table) {}
            public TestStatus TestStatus { get { return new TestStatus(); } }
            public void Prepare(CellProcessor processor, Interpreter parent, Tree<Cell> table) {
            }
        }

        private class SampleItem: Copyable, SetUpTearDown {
            public bool IsSetUp;
            public bool IsTearDown;
            public Copyable Copy() {
                throw new NotImplementedException();
            }

            public void SetUp() {
                IsSetUp = true;
            }

            public void TearDown() {
                IsTearDown = true;
            }
        }
    }
}
