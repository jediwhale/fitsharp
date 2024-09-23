// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class RunTestDefaultTest {
        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
            processor.AddOperator(new TestParseInterpreter());
            processor.AddOperator(new TestInvokeSpecialAction(), 2);
            runTest = new RunTestDefault();
            tables = new CellTree(new CellTree(new CellTree("myfixture")));
        }
    
        [Test] public void CreatesFixtureOnce() {
            SampleFixture.Count = 0;
            RunTest();
            ClassicAssert.AreEqual(1, SampleFixture.Count);
        }

        [Test] public void SetsUpConfiguration() {
            processor.Configuration.GetItem<SampleItem>();
            RunTest();
            ClassicAssert.IsTrue(processor.Configuration.GetItem<SampleItem>().IsSetUp);
        }

        [Test] public void TearsDownConfiguration() {
            processor.Configuration.GetItem<SampleItem>();
            RunTest();
            ClassicAssert.IsTrue(processor.Configuration.GetItem<SampleItem>().IsTearDown);
        }

        void RunTest() {
            runTest.Run(processor, tables, writer);
        }

        class TestParseInterpreter: CellOperator, ParseOperator<Cell> {
            public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
                return typeof (Interpreter).IsAssignableFrom(type);
            }

            public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
                return new TypedValue(new SampleFixture());
            }
        }

        CellProcessorBase processor;
        CellTree tables;
        RunTestDefault runTest;
        readonly StoryTestNullWriter writer = new StoryTestNullWriter();

        class TestInvokeSpecialAction: CellOperator, InvokeSpecialOperator {
            public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
                return true;
            }

            public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
                return TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName.Name, 1));
            }
         }

        class SampleFixture: Interpreter {
            public static int Count;
            public SampleFixture() { Count++; }
            public void Interpret(CellProcessor processor, Tree<Cell> table) {}
        }

        class SampleItem: Copyable, SetUpTearDown {
            public bool IsSetUp;
            public bool IsTearDown;
            public Copyable Copy() { throw new NotImplementedException(); }
            public void SetUp() { IsSetUp = true; }
            public void TearDown() { IsTearDown = true; }
        }
    }
}
