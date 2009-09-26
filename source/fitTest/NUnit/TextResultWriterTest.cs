// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitnesse.fitserver;
using fitSharp.Fit.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class TextResultWriterTest
    {
        private const string TEST_RESULT_FILE_NAME = "Test.result";
        private TextResultWriter _strategy;
        private FolderTestModel _folderModel;

        [SetUp]
        public void SetUp()
        {
            _folderModel = new FolderTestModel();
        }

        [Test]
        public void TestCloseWithFileName()
        {
            _strategy = new TextResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.Close();
            Assert.IsTrue(_folderModel.FileExists(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestCloseWithStandardOut()
        {
            _strategy = new TextResultWriter("stdout", _folderModel);
            _strategy.Close();
            Assert.IsFalse(_folderModel.FileExists(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestWriteResults()
        {
            var pageResult = new PageResult("Test Page", "content", TestUtils.MakeTestStatus());
            _strategy = new TextResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.WritePageResult(pageResult);
            _strategy.Close();
            Assert.AreEqual("0000000060Test Page\n1 right, 2 wrong, 3 ignored, 4 exceptions\ncontent\n", _folderModel.FileContent(TEST_RESULT_FILE_NAME));
        }

        [Test]
        public void TestWriteFinalCounts()
        {
            _strategy = new TextResultWriter(TEST_RESULT_FILE_NAME, _folderModel);
            _strategy.WriteFinalCount(TestUtils.MakeTestStatus());
            _strategy.Close();
            Assert.AreEqual("00000000000000000001000000000200000000030000000004", _folderModel.FileContent(TEST_RESULT_FILE_NAME));
        }
    }
}