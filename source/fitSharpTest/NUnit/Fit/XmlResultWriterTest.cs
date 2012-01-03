// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Test.Double;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class XmlResultWriterTest
    {
        const string TEST_RESULT_FILE_NAME = "Test.xml";
        XmlResultWriter _strategy;
        FolderTestModel _folderModel;

        [SetUp]
        public void SetUp()
        {
            _folderModel = new FolderTestModel();
        }

        [Test]
        public void TestCloseWithFileName()
        {
            _strategy = new XmlResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.Close();
            Assert.IsTrue(_folderModel.FileExists(TEST_RESULT_FILE_NAME));
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<testResults />", _folderModel.GetPageContent(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestCloseWithStandardOut()
        {
            _strategy = new XmlResultWriter("stdout", _folderModel);
            _strategy.Close();
            Assert.IsFalse(_folderModel.FileExists(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestWriteResults()
        {
            const string pageName = "Test Page";
            var pageResult = new PageResult(pageName, "<table border=\"1\" cellspacing=\"0\">\r\n<tr><td>Text</td>\r\n</tr>\r\n</table>", MakeTestCounts());
            _strategy = new XmlResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.WritePageResult(pageResult);
            _strategy.Close();
            Assert.AreEqual(
                BuildPageResultString(pageName, "<![CDATA[<table border=\"1\" cellspacing=\"0\">\r\n<tr><td>Text</td>\r\n</tr>\r\n</table>]]>", 1, 2, 3, 4),
                _folderModel.GetPageContent(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestWriteIllegalCharacters()
        {
            const string pageName = "Test Page";
            var pageResult = new PageResult(pageName, "<table><tr><td>Text</td></tr>\x02</table>", MakeTestCounts());
            _strategy = new XmlResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.WritePageResult(pageResult);
            _strategy.Close();
            Assert.AreEqual(
                BuildPageResultString(pageName, "<![CDATA[<table><tr><td>Text</td></tr>&#2;</table>]]>", 1, 2, 3, 4),
                _folderModel.GetPageContent(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestWriteFinalCounts()
        {
            _strategy = new XmlResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.WriteFinalCount(MakeTestCounts());
            _strategy.Close();
            Assert.AreEqual(BuildFinalCountsString(1, 2, 3, 4),
                            _folderModel.GetPageContent(TEST_RESULT_FILE_NAME));
        }

        static string BuildPageResultString(string pageName, string content, int right, int wrong, int ignores, int exceptions)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
            builder.AppendLine("<testResults>");
            builder.AppendLine("  <result>");
            builder.AppendFormat("    <relativePageName>{0}</relativePageName>\r\n", pageName);
            builder.AppendFormat("    <content>{0}</content>\r\n", content);
            builder.AppendLine("    <counts>");
            builder.AppendFormat("      <right>{0}</right>\r\n", right);
            builder.AppendFormat("      <wrong>{0}</wrong>\r\n", wrong);
            builder.AppendFormat("      <ignores>{0}</ignores>\r\n", ignores);
            builder.AppendFormat("      <exceptions>{0}</exceptions>\r\n", exceptions);
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
            builder.AppendFormat("    <right>{0}</right>\r\n", right);
            builder.AppendFormat("    <wrong>{0}</wrong>\r\n", wrong);
            builder.AppendFormat("    <ignores>{0}</ignores>\r\n", ignores);
            builder.AppendFormat("    <exceptions>{0}</exceptions>\r\n", exceptions);
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
