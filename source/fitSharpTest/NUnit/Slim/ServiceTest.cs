// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Operators;
using fitSharp.Slim.Service;
using fitSharp.Test.Double.Slim;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ServiceTest {
        Service service;

        [SetUp] public void SetUp() {
            service = Builder.Service();
        }

        [Test] public void InstanceIsCreated() {
            SampleClass.Count = 0;
            var statement = new Instructions().MakeVariable("variable", typeof (SampleClass));
            DoInstruction(statement);
            Assert.AreEqual(1, SampleClass.Count);
        }

        private TypedValue DoInstruction(Instructions statement) {
            return service.Invoke(new TypedValue(new SlimInstruction()), new MemberName(string.Empty), statement.Tree.Branches[0]);
        }

        [Test] public void OperatorIsAddedFromConfiguration() {
            var configuration = new TypeDictionary();
            new SuiteConfiguration(configuration).LoadXml("<config><fitSharp.Slim.Service.Service><addOperator>fitSharp.Test.NUnit.Slim.SampleOperator</addOperator></fitSharp.Slim.Service.Service></config>");
            var statement = new Instructions().MakeCommand("sampleCommand");
            service = new Service(configuration);
            var result = DoInstruction(statement).GetValue<Tree<string>>();
            Assert.AreEqual("sampleResult", result.ValueAt(1));
        }

        [Test] public void ParseSymbolIsDoneFirst() {
            service.Get<Symbols>().Save("symbol", "input");
            service.AddOperator(new SampleConverter());
            var value = (SampleClass)service.Parse(typeof(SampleClass), TypedValue.Void, new SlimLeaf("$symbol")).Value;
            Assert.AreEqual("custominput", value.Info);
        }

        [Test] public void CustomComposeIsCalled() {
            service.AddOperator(new SampleConverter());
            var statement = new Instructions().MakeVariable("variable", typeof(SampleClass));
            DoInstruction(statement);
            statement = new Instructions().ExecuteMethod("makesample");
            var result = DoInstruction(statement).GetValue<SlimTree>();
            Assert.AreEqual("mysample", result.ValueAt(1));
        }

        class SampleConverter: Converter<SampleClass> {
            protected override SampleClass Parse(string input) {
                return new SampleClass {Info = ("custom" + input)};
            }

            protected override string Compose(SampleClass input) {
                return "my" + input.Info;
            }
        }
    }

    public class SampleOperator: InvokeInstructionBase {
        public SampleOperator() : base("sampleCommand") {}
        protected override Tree<string> ExecuteOperation(Tree<string> parameters) {
            return Result(parameters, "sampleResult");
        }
    }
}