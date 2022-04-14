// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Slim.Analysis;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture]
    public class ReportTest {
        [Test]
        public void EmptyInputHasEmptyOutput() {
            AssertReport(string.Empty, string.Empty);
        }
        
        [Test]
        public void SinglePageIsReported() {
            AssertReport(pageInstructions1, pageReport1);
        }

        [Test]
        public void TwoPagesAreReported() {
            AssertReport(pageInstructions1 + pageInstructions2, pageReport1 + pageReport2);
        }

        [Test]
        public void MultilinePageIsReported() {
            AssertReport(multilineInstructions, pageReport2);
        }

        static void AssertReport(string input, string expected) {
            var output = new StringBuilder();
            var applicationUnderTest = new ApplicationUnderTest();
            applicationUnderTest.AddNamespace("fitSharp.Test.NUnit.Slim");
            Report.RunReport(applicationUnderTest, new StringReader(input), new StringWriter(output));
            Assert.AreEqual(expected, output.ToString());
        }

        static readonly string pageInstructions1 = "MyPageOne|[000002:000087:[000004:000015:scriptTable_0_0:000004:make:000016:scriptTableActor:000011:SampleClass:]:000088:[000004:000015:scriptTable_0_1:000004:call:000016:scriptTableActor:000012:sampleMethod:]:]" + Environment.NewLine;
        static readonly string pageInstructions2 = "MyPageTwo|[000002:000096:[000005:000015:scriptTable_0_0:000004:make:000016:scriptTableActor:000011:SampleClass:000001:a:]:000107:[000005:000015:scriptTable_0_1:000004:call:000016:scriptTableActor:000020:sampleMethodWithParm:000003:now:]:]" + Environment.NewLine;
        static readonly string multilineInstructions = "MyPageTwo|[000002:000096:[000005:000015:scriptTable_0_0:000004:make:000016:scriptTableActor:000011:SampleClass:000001:a:]:000112:[000005:000015:scriptTable_0_1:000004:call:000016:scriptTableActor:000020:sampleMethodWithParm:000008:new%0Dline:]:]" + Environment.NewLine;

        static readonly string pageReport1 = "MyPageOne|fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(0)" + Environment.NewLine +
                                             "MyPageOne|fitSharp.Test.NUnit.Slim.SampleClass:SampleMethod(0)" + Environment.NewLine;

        static readonly string pageReport2 = "MyPageTwo|fitSharp.Test.NUnit.Slim.SampleClass:SampleClass(1)"  + Environment.NewLine + 
                                             "MyPageTwo|fitSharp.Test.NUnit.Slim.SampleClass:SampleMethodWithParm(1)" + Environment.NewLine;
    }
}