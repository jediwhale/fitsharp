// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Operators;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ServiceTest {
        Service service;

        [SetUp] public void SetUp() {
            service = new Service();
        }

        [Test] public void InstanceIsCreated() {
            SampleClass.Count = 0;
            var statement = new Instructions().MakeVariable("variable", typeof (SampleClass));
            service.Execute(TypedValue.Void, statement.Tree.Branches[0]);
            Assert.AreEqual(1, SampleClass.Count);
        }

        [Test] public void OperatorIsAddedFromConfiguration() {
            var configuration = new Configuration();
            configuration.LoadXml("<config><fitSharp.Slim.Service.Service><addOperator>fitSharp.Test.NUnit.Slim.SampleOperator</addOperator></fitSharp.Slim.Service.Service></config>");
            var statement = new SlimTree().AddBranchValue("step").AddBranchValue("sampleCommand");
            var result = new Service(configuration).Execute(TypedValue.Void, statement).GetValue<Tree<string>>();
            Assert.AreEqual("sampleResult", result.Branches[1].Value);
        }

        [Test] public void ParseSymbolIsDoneFirst() {
            service.Store(new Symbol("symbol", "input"));
            service.AddOperator(new SampleConverter());
            var value = (SampleClass)service.Parse(typeof(SampleClass), TypedValue.Void, new SlimLeaf("$symbol")).Value;
            Assert.AreEqual("custominput", value.Info);
        }

        [Test] public void CustomComposeIsCalled() {
            service.AddOperator(new SampleConverter());
            var statement = new Instructions().MakeVariable("variable", typeof(SampleClass));
            service.Execute(TypedValue.Void, statement.Tree.Branches[0]);
            statement = new Instructions().ExecuteMethod("makesample");
            var result = service.Execute(TypedValue.Void, statement.Tree.Branches[0]).GetValue<SlimTree>();
            Assert.AreEqual("mysample", result.Branches[1].Value);
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

    public class SampleOperator: ExecuteBase {
        public SampleOperator() : base("sampleCommand") {}
        protected override Tree<string> ExecuteOperation(Tree<string> parameters) {
            return Result(parameters, "sampleResult");
        }
    }
}