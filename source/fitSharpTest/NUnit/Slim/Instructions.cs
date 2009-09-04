// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Test.NUnit.Slim
{
    class Instructions
    {
        public static Tree<string> MakeSampleClass() {
            return new TreeList<string>().AddBranchValue("step1").AddBranchValue("make").AddBranchValue("variable").
                AddBranchValue(typeof (SampleClass).FullName);
        }

        public static Tree<string> ExecuteMethod(string methodName) {
            return new TreeList<string>().AddBranchValue("step2").AddBranchValue("call").AddBranchValue("variable").
                AddBranchValue(methodName);
        }

        public static Tree<string> ExecuteAbortTest() {
            return new TreeList<string>().AddBranchValue("step3").AddBranchValue("call").AddBranchValue("variable").
                AddBranchValue("aborttest");
        }

    }
}
