// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Slim.Analysis;
using fitSharp.Slim.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture]
    public class AnalyzerTest {
        [Test]
        public void ProcessMakeFindsType() {
            Process("make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass");
            AssertCalls("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0)");
        }

        [Test]
        public void ProcessMakeReportsMissingType() {
            Process("make", "instance0", "garbage");
            AssertCalls("* Type 'garbage' not found *");
        }

        [Test]
        public void ProcessMakeFindsGracefulType() {
            Process("import", "fitSharp.Test.NUnit.Slim");
            Process("make", "instance1", "sample class");
            AssertCalls("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0)");
        }

        [Test]
        public void ProcessCallFindsMethod([ValueSource(nameof(callCommands))] string[] command) {
            Process("make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass");
            ProcessCall(command, "instance0", "SampleMethod");
            AssertCalls("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0),"
                        + "fitSharp.Test.NUnit.Slim.SampleClass:SampleMethod(0)");
        }

        [Test]
        public void ProcessCallFindsLibraryMethod([ValueSource(nameof(callCommands))] string[] command) {
            Process("make", "libraryInstance", "fitSharp.Test.NUnit.Slim.SampleClass");
            Process("make", "instance0", "fitSharp.Test.NUnit.Slim.OtherSampleClass");
            ProcessCall(command, "instance0", "SampleMethod");
            AssertCalls("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0),"
                        + "fitSharp.Test.NUnit.Slim.OtherSampleClass:OtherSampleClass(0),"
                        + "fitSharp.Test.NUnit.Slim.SampleClass:SampleMethod(0)");
        }
        
        [Test]
        public void ProcessCallReportsMissingMethod([ValueSource(nameof(callCommands))] string[] command) {
            Process("make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass");
            ProcessCall(command, "instance0", "garbage");
            AssertCalls("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0),"
                        + "* Method 'garbage' not found *");
        }

        [Test]
        public void ProcessCallReportsMissingInstance([ValueSource(nameof(callCommands))] string[] command) {
            ProcessCall(command, "instance0", "SampleMethod");
            AssertCalls("* Unknown type for method 'SampleMethod' *");
        }

        [Test]
        public void ProcessCallFindsGracefulMethod([ValueSource(nameof(callCommands))] string[] command) {
            Process("make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass");
            ProcessCall(command, "instance0", "sample method");
            AssertCalls("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0),"
                        + "fitSharp.Test.NUnit.Slim.SampleClass:SampleMethod(0)");
        }

        [Test]
        public void ProcessAssignDoesNothing() {
            Process("assign", "symbol0", "value0");
            AssertCalls(string.Empty);
        }

        [SetUp]
        public void SetUp() {
            analyzer = new Analyzer();
            id = 0;
        }

        void AssertCalls(string expected) {
            ClassicAssert.AreEqual(expected, string.Join(",", analyzer.Calls));
        }

        void Process(params object[] items) {
            var tree = new SlimTree();
            tree.AddBranchValue("id" + id++);
            analyzer.Process(tree.AddBranches(items));
        }

        void ProcessCall(IEnumerable<string> commands, params object[] items) {
            var tree = new SlimTree();
            tree.AddBranchValue("id" + id++);
            foreach (var command in commands) tree.AddBranchValue(command);
            analyzer.Process(tree.AddBranches(items));
        }

        static readonly string[] callCommand = {"call"};
        static readonly string[] callAndAssignCommand = {"callAndAssign", "symbol0"};
        static readonly string[][] callCommands = {callCommand, callAndAssignCommand};
        
        Analyzer analyzer;
        int id;
    }
}