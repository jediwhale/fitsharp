// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Slim.Model;

namespace fitSharp.Test.NUnit.Slim
{
    class Instructions
    {
        private readonly SlimTree instructionTree = new SlimTree();

        public SlimTree Tree { get { return instructionTree; } }

        public Instructions MakeVariable(string variableName, Type variableType) {
            instructionTree.AddBranch(
                new SlimTree().AddBranchValue("step1").AddBranchValue("make").AddBranchValue(variableName).
                    AddBranchValue(variableType.FullName));
            return this;
        }

        public Instructions MakeCommand(string commandName) {
            instructionTree.AddBranch(
                new SlimTree().AddBranchValue("stepx").AddBranchValue(commandName));
            return this;
        }


        public Instructions ExecuteMethod(string methodName) {
            instructionTree.AddBranch(
                new SlimTree().AddBranchValue("step2").AddBranchValue("call").AddBranchValue("variable").
                    AddBranchValue(methodName));
            return this;
        }

        public Instructions ExecuteAbortTest() {
            instructionTree.AddBranch(
                new SlimTree().AddBranchValue("step3").AddBranchValue("call").AddBranchValue("variable").
                    AddBranchValue("aborttest"));
            return this;
        }

        public void Execute() {
            string instructionString = new fitSharp.Slim.Service.Document(instructionTree).ToString();
            TestInterpreter.ExecuteInstructions(instructionString);
        }

    }
}
