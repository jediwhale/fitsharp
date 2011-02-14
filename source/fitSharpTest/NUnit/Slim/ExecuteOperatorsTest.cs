// Copyright © 2011 Syterra Software Inc. All rights reserved.
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
    [TestFixture] public class ExecuteOperatorsTest {
        private Service processor;
        private Tree<string> result;

        [SetUp] public void SetUp() {
            processor = new Service();
        }

        [Test] public void ExecuteDefaultReturnsException() {
            var executeDefault = new ExecuteDefault { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("garbage");
            ExecuteOperation(executeDefault, input, 2);
            CheckForException("message:<<MALFORMED_INSTRUCTION step,garbage>>");
        }

        [Test] public void ExecuteMakeBadClassReturnsException() {
            var executeMake = new ExecuteMake { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("make").AddBranchValue("variable").AddBranchValue("garbage");
            ExecuteOperation(executeMake, input, 2);
            CheckForException("message:<<NO_CLASS garbage>>");
        }

        [Test] public void ExecuteMakeUsesSymbolInClassName() {
            processor.Store(new Symbol("symbol", "NUnit"));
            var executeMake = new ExecuteMake { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("make").AddBranchValue("variable").AddBranchValue("fitSharp.Test.$symbol.Slim.SampleClass");
            ExecuteOperation(executeMake, input, 2);
            Assert.IsTrue(processor.Load(new SavedInstance("variable")).Instance is SampleClass);
        }

        [Test] public void ExecuteMakeUsesSymbolAsObject() {
            var newClass = new SampleClass();
            processor.Store(new Symbol("symbol", newClass));
            var executeMake = new ExecuteMake { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("make").AddBranchValue("variable").AddBranchValue("$symbol");
            ExecuteOperation(executeMake, input, 2);
            Assert.AreEqual(newClass, processor.Load(new SavedInstance("variable")).Instance);
        }

        [Test] public void ExecuteMakeLibraryIsStacked() {
            var executeMake = new ExecuteMake { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("make").AddBranchValue("librarystuff").AddBranchValue("fitSharp.Test.NUnit.Slim.SampleClass");
            ExecuteOperation(executeMake, input, 2);
            foreach (TypedValue libraryInstance in processor.LibraryInstances) {
                Assert.IsTrue(libraryInstance.Value is SampleClass);
                return;
            }
            Assert.Fail();
        }

        [Test] public void ExecuteCallBadMethodReturnsException() {
            processor.Store(new SavedInstance("variable", new SampleClass()));
            var executeCall = new ExecuteCall { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("call").AddBranchValue("variable").AddBranchValue("garbage");
            ExecuteOperation(executeCall, input, 2);
            CheckForException("message:<<NO_METHOD_IN_CLASS garbage fitSharp.Test.NUnit.Slim.SampleClass>>");
        }

        [Test] public void ExecuteImportAddsNamespace() {
            var executeImport = new ExecuteImport { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("import").AddBranchValue("fitSharp.Test.NUnit.Slim");
            ExecuteOperation(executeImport, input, 2);
            Assert.IsTrue(processor.Create("SampleClass", new SlimTree()).Value is SampleClass);
        }

        [Test] public void ExecuteCallAndAssignSavesSymbol() {
            processor.Store(new SavedInstance("variable", new SampleClass()));
            var executeCallAndAssign = new ExecuteCallAndAssign { Processor = processor };
            var input =
                new SlimTree().AddBranchValue("step").AddBranchValue("callAndAssign").AddBranchValue("symbol").AddBranchValue(
                    "variable").AddBranchValue("sampleMethod");
            ExecuteOperation(executeCallAndAssign, input, 2);
            Assert.AreEqual("testresult", result.Branches[1].Value);
            Assert.AreEqual("testresult", processor.Load(new Symbol("symbol")).Instance);
        }

        [Test] public void ExecuteCallUsesDomainAdapter() {
            processor.Store(new SavedInstance("variable", new SampleClass()));
            var executeCall = new ExecuteCall { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("call").AddBranchValue("variable").AddBranchValue("DomainMethod");
            ExecuteOperation(executeCall, input, 2);
            Assert.AreEqual("domainstuff", result.Branches[1].Value);
        }

        [Test] public void ExecuteCallOnMissingInstanceUsesLibrary() {
            var executeMake = new ExecuteMake { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("make").AddBranchValue("librarystuff").AddBranchValue("fitSharp.Test.NUnit.Slim.SampleClass");
            ExecuteOperation(executeMake, input, 2);
            var executeCall = new ExecuteCall { Processor = processor };
            input = new SlimTree().AddBranchValue("step").AddBranchValue("call").AddBranchValue("garbage").AddBranchValue("SampleMethod");
            SampleClass.MethodCount = 0;
            ExecuteOperation(executeCall, input, 2);
            Assert.AreEqual(1, SampleClass.MethodCount);
        }

        [Test] public void ExecuteGetFixtureReturnsActorInstance() {
            MakeSampleClass("sampleData");
            CallActorMethod("getFixture");
            Assert.AreEqual("Sample=sampleData", result.Branches[1].Value);
        }

        [Test] public void ExecutePushAndPopFixtureReturnsActorInstance() {
            MakeSampleClass("sampleData");
            CallActorMethod("pushFixture");
            MakeSampleClass("otherData");
            CallActorMethod("popFixture");
            CallActorMethod("info");
            Assert.AreEqual("sampleData", result.Branches[1].Value);
        }

        private void MakeSampleClass(string sampleData) {
            var executeMake = new ExecuteMake { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("make").AddBranchValue("scriptTableActor").AddBranchValue("fitSharp.Test.NUnit.Slim.SampleClass").AddBranchValue(sampleData);
            ExecuteOperation(executeMake, input, 2);
        }

        private void CallActorMethod(string methodName) {
            var executeCall = new ExecuteCall { Processor = processor };
            var input = new SlimTree().AddBranchValue("step").AddBranchValue("call").AddBranchValue("scriptTableActor").AddBranchValue(methodName);
            ExecuteOperation(executeCall, input, 2);
        }

        private void ExecuteOperation(ExecuteOperator<string> executeOperator, Tree<string> input, int branchCount) {
            TypedValue executeResult = TypedValue.Void;
            if (executeOperator.CanExecute(TypedValue.Void, input)) {
                executeResult = executeOperator.Execute(TypedValue.Void, input);
            }
            result = executeResult.GetValue<Tree<string>>();
            Assert.IsFalse(result.IsLeaf);
            Assert.AreEqual(branchCount, result.Branches.Count);
            Assert.AreEqual("step", result.Branches[0].Value);
        }

        private void CheckForException(string exceptionText) {
            Assert.IsTrue(result.Branches[1].Value.StartsWith("__EXCEPTION__:" + exceptionText));
        }
    }
}