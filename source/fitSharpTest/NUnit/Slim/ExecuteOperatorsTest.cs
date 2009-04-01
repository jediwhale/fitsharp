// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ExecuteOperatorsTest {
        private Processor<string> processor;
        private Tree<string> result;

        [SetUp] public void SetUp() {
            processor = new Processor<string>();
            processor.AddOperator(new ComposeDefault());
            processor.AddOperator(new ComposeException());
            processor.AddMemory<SavedInstance>();
            processor.AddMemory<Symbol>();
        }

        [Test] public void ExecuteDefaultReturnsException() {
            var executeDefault = new ExecuteDefault();
            var input = new TreeList<string>().AddBranchValue("step").AddBranchValue("garbage");
            ExecuteOperation(executeDefault, input, 2);
            CheckForException("message:<<MALFORMED_INSTRUCTION step,garbage>>");
        }

        [Test] public void ExecuteMakeBadClassReturnsException() {
            var executeMake = new ExecuteMake();
            var input = new TreeList<string>().AddBranchValue("step").AddBranchValue("make").AddBranchValue("variable").AddBranchValue("garbage");
            ExecuteOperation(executeMake, input, 2);
            CheckForException("message:<<NO_CLASS garbage>>");
        }

        [Test] public void ExecuteCallBadMethodReturnsException() {
            processor.Store(new SavedInstance("variable", new SampleClass()));
            var executeCall = new ExecuteCall();
            var input = new TreeList<string>().AddBranchValue("step").AddBranchValue("call").AddBranchValue("variable").AddBranchValue("garbage");
            ExecuteOperation(executeCall, input, 2);
            CheckForException("message:<<NO_METHOD_IN_CLASS garbage fitSharp.Test.NUnit.Slim.SampleClass>>");
        }

        [Test] public void ExecuteImportAddsNamespace() {
            var executeImport = new ExecuteImport();
            var input = new TreeList<string>().AddBranchValue("step").AddBranchValue("import").AddBranchValue("fitSharp.Test.NUnit.Slim");
            ExecuteOperation(executeImport, input, 2);
            Assert.IsTrue(processor.Create("SampleClass").Value is SampleClass);
        }

        [Test] public void ExecuteCallAndAssignSavesSymbol() {
            processor.Store(new SavedInstance("variable", new SampleClass()));
            var executeCallAndAssign = new ExecuteCallAndAssign();
            var input =
                new TreeList<string>().AddBranchValue("step").AddBranchValue("callAndAssign").AddBranchValue("symbol").AddBranchValue(
                    "variable").AddBranchValue("sampleMethod");
            ExecuteOperation(executeCallAndAssign, input, 2);
            Assert.AreEqual("testresult", result.Branches[1].Value);
            Assert.AreEqual("testresult", processor.Load(new Symbol("symbol")).Instance);
        }

        private void ExecuteOperation(ExecuteOperator<string> executeOperator, Tree<string> input, int branchCount) {
            TypedValue executeResult = TypedValue.Void;
            executeOperator.TryExecute(processor, TypedValue.Void, input, ref executeResult);
            result = (Tree<string>)executeResult.Value;
            Assert.IsFalse(result.IsLeaf);
            Assert.AreEqual(branchCount, result.Branches.Count);
            Assert.AreEqual("step", result.Branches[0].Value);
        }

        private void CheckForException(string exceptionText) {
            Assert.IsTrue(result.Branches[1].Value.StartsWith("__EXCEPTION__:" + exceptionText));
        }
    }
}