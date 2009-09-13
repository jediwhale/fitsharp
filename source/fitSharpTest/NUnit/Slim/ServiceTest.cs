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
            var statement = Instructions.MakeSampleClass();
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
            var value = service.Parse(typeof(string), TypedValue.Void, new TreeLeaf<string>("$symbol")).Value;
            Assert.AreEqual("TESTVALUE", value);
        }

        private class ParseUpperCase: SlimOperator, ParseOperator<string> {
            public bool CanParse(Type type, TypedValue instance, Tree<string> parameters) {
                return true;
            }

            public TypedValue Parse(Type type, TypedValue instance, Tree<string> parameters) {
                return new TypedValue(parameters.Value.ToUpper());
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