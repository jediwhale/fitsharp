// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Slim.Model;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture]
    public class AnalyzerTest {
        [Test]
        public void ProcessMakeFindsType() {
            var analyzer = new Analyzer();
            analyzer.Process(new SlimTree().AddBranches("id0", "make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass"));
            Assert.AreEqual("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0)", string.Join(",", analyzer.Calls));
        }
        
        [Test]
        public void ProcessMakeFindsGracefulType() {
            var analyzer = new Analyzer();
            analyzer.Process(new SlimTree().AddBranches("id0", "import", "fitSharp.Test.NUnit.Slim"));
            analyzer.Process(new SlimTree().AddBranches("id1", "make", "instance1", "sample class"));
            Assert.AreEqual("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0)", string.Join(",", analyzer.Calls));
        }

        [Test]
        public void ProcessCallFindsMethod() {
            var analyzer = new Analyzer();
            analyzer.Process(new SlimTree().AddBranches("id0", "make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass"));
            analyzer.Process(new SlimTree().AddBranches("id1", "call", "instance0", "SampleMethod"));
            Assert.AreEqual("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0),"
                            + "fitSharp.Test.NUnit.Slim.SampleClass:SampleMethod(0)", string.Join(",", analyzer.Calls));
        }

        [Test]
        public void ProcessCallAndAssignFindsMethod() {
            var analyzer = new Analyzer();
            analyzer.Process(new SlimTree().AddBranches("id0", "make", "instance0", "fitSharp.Test.NUnit.Slim.SampleClass"));
            analyzer.Process(new SlimTree().AddBranches("id1", "callAndAssign", "symbol0", "instance0", "SampleMethod"));
            Assert.AreEqual("fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0),"
                            + "fitSharp.Test.NUnit.Slim.SampleClass:SampleMethod(0)", string.Join(",", analyzer.Calls));
        }

        [Test]
        public void ProcessAssignDoesNothing() {
            var analyzer = new Analyzer();
            analyzer.Process(new SlimTree().AddBranches("id0", "assign", "symbol0", "value0"));
            Assert.AreEqual(string.Empty, string.Join(",", analyzer.Calls));
        }
    }
}