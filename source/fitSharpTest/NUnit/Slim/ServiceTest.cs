// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ServiceTest {
        private Service service;

        [SetUp] public void SetUp() {
            service = new Service();
        }

        [Test] public void InstanceIsCreated() {
            SampleClass.Count = 0;
            var statement =
                new TreeList<string>().AddBranchValue("step").AddBranchValue("make").AddBranchValue("variable").AddBranchValue(
                    typeof(SampleClass).FullName);
            service.Execute(statement);
            Assert.AreEqual(1, SampleClass.Count);
        }

        [Test] public void OperatorIsAddedFromConfiguration() {
            var configuration = new Configuration();
            configuration.LoadXml("<config><fitSharp.Slim.Service.Service><addOperator>fitSharp.Test.NUnit.Slim.SampleOperator</addOperator></fitSharp.Slim.Service.Service></config>");
            var statement = new TreeList<string>().AddBranchValue("step").AddBranchValue("sampleCommand");
            var result = (Tree<string>)configuration.GetItem<Service>().Execute(statement).Value;
            Assert.AreEqual("sampleResult", result.Branches[1].Value);
        }

        [Test] public void ParseSymbolIsDoneFirst() {
            service.Store(new Symbol("symbol", "testvalue"));
            service.AddOperator(new ParseUpperCase());
            var value = service.Parse<string>("$symbol");
            Assert.AreEqual("TESTVALUE", value);
        }

        private class ParseUpperCase: ParseOperator<string> {
            public bool TryParse(Processor<string> processor, Type type, TypedValue instance, Tree<string> parameters, ref TypedValue result) {
                result = new TypedValue(parameters.Value.ToUpper());
                return true;
            }
        }
    }

    public class SampleOperator: ExecuteBase {
        public SampleOperator() : base("sampleCommand") {}
        protected override Tree<string> ExecuteOperation(Processor<string> processor, Tree<string> parameters) {
            return Result(parameters, "sampleResult");
        }
    }
}