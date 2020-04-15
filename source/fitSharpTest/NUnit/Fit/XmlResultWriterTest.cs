// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Samples;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class XmlResultWriterTest
    {
        const string testResultFileName = "Test.xml";
        XmlResultWriter strategy;
        FolderTestModel folderModel;

        [SetUp]
        public void SetUp()
        {
            folderModel = new FolderTestModel();
        }

        [Test]
        public void TestCloseWithFileName()
        {
            strategy = new XmlResultWriter(testResultFileName, folderModel);
            strategy.Close();
            Assert.IsTrue(folderModel.Exists(testResultFileName));
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine + "<testResults />", folderModel.GetPageContent(testResultFileName));
        }

        [Test]
        public void TestCloseWithStandardOut()
        {
            strategy = new XmlResultWriter("stdout", folderModel);
            strategy.Close();
            Assert.IsFalse(folderModel.Exists(testResultFileName));
        }

        [Test]
        public void TestWriteResults()
        {
            const string pageName = "Test Page";
            var pageResult = new PageResult(pageName, "<table border=\"1\" cellspacing=\"0\">" + Environment.NewLine
                       + "<tr><td>Text</td>" + Environment.NewLine 
                       + "</tr>" + Environment.NewLine + "</table>", MakeTestCounts());
            strategy = new XmlResultWriter(testResultFileName, folderModel);
            strategy.WritePageResult(pageResult);
            strategy.Close();
            Assert.AreEqual(
                BuildPageResultString(pageName, "<![CDATA[<table border=\"1\" cellspacing=\"0\">" + Environment.NewLine
                          + "<tr><td>Text</td>" + Environment.NewLine 
                          + "</tr>" + Environment.NewLine
                          + "</table>]]>", 1, 2, 3, 4),
                folderModel.GetPageContent(testResultFileName));
        }

        [Test]
        public void TestWriteIllegalCharacters()
        {
            const string pageName = "Test Page";
            var pageResult = new PageResult(pageName, "<table><tr><td>Text</td></tr>\x02</table>", MakeTestCounts());
            strategy = new XmlResultWriter(testResultFileName, folderModel);
            strategy.WritePageResult(pageResult);
            strategy.Close();
            Assert.AreEqual(
                BuildPageResultString(pageName, "<![CDATA[<table><tr><td>Text</td></tr>&#2;</table>]]>", 1, 2, 3, 4),
                folderModel.GetPageContent(testResultFileName));
        }

        [Test]
        public void TestWriteFinalCounts()
        {
            strategy = new XmlResultWriter(testResultFileName, folderModel);
            strategy.WriteFinalCount(MakeTestCounts());
            strategy.Close();
            Assert.AreEqual(BuildFinalCountsString(1, 2, 3, 4),
                            folderModel.GetPageContent(testResultFileName));
        }

        static string BuildPageResultString(string pageName, string content, int right, int wrong, int ignores, int exceptions)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
            builder.AppendLine("<testResults>");
            builder.AppendLine("  <result>");
            builder.AppendFormat("    <relativePageName>{0}</relativePageName>" + Environment.NewLine, pageName);
            builder.AppendFormat("    <content>{0}</content>" + Environment.NewLine, content);
            builder.AppendLine("    <counts>");
            builder.AppendFormat("      <right>{0}</right>" + Environment.NewLine, right);
            builder.AppendFormat("      <wrong>{0}</wrong>" + Environment.NewLine, wrong);
            builder.AppendFormat("      <ignores>{0}</ignores>" + Environment.NewLine, ignores);
            builder.AppendFormat("      <exceptions>{0}</exceptions>" + Environment.NewLine, exceptions);
            builder.AppendLine("    </counts>");
            builder.AppendLine("  </result>");
            builder.Append("</testResults>");
            return builder.ToString();
        }

        static string BuildFinalCountsString(int right, int wrong, int ignores, int exceptions)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
            builder.AppendLine("<testResults>");
            builder.AppendLine("  <finalCounts>");
            builder.AppendFormat("    <right>{0}</right>" + Environment.NewLine, right);
            builder.AppendFormat("    <wrong>{0}</wrong>" + Environment.NewLine, wrong);
            builder.AppendFormat("    <ignores>{0}</ignores>" + Environment.NewLine, ignores);
            builder.AppendFormat("    <exceptions>{0}</exceptions>" + Environment.NewLine, exceptions);
            builder.AppendLine("  </finalCounts>");
            builder.Append("</testResults>");
            return builder.ToString();
        }

        static TestCounts MakeTestCounts() {
            var counts = new TestCounts();
            counts.AddCount(TestStatus.Right);
            counts.AddCount(TestStatus.Wrong);
            counts.AddCount(TestStatus.Wrong);
            counts.AddCount(TestStatus.Ignore);
            counts.AddCount(TestStatus.Ignore);
            counts.AddCount(TestStatus.Ignore);
            counts.AddCount(TestStatus.Exception);
            counts.AddCount(TestStatus.Exception);
            counts.AddCount(TestStatus.Exception);
            counts.AddCount(TestStatus.Exception);
            return counts;
        }
    }
}
