// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using fitnesse.fitserver;
using fitSharp.Fit.Service;
using fitSharp.Machine.Application;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class TestRunnerTest
    {
        private TestRunner runner;
        private const string TEST_FILE_NAME = "TestFile.xml";
        private Configuration configuration;

        [SetUp]
        public void SetUp()
        {
            runner = new TestRunner();
            configuration = new Configuration();
        }

        [TearDown]
        public void TearDown()
        {
            if(runner.resultWriter != null)
                runner.resultWriter.Close();
            if (File.Exists(TEST_FILE_NAME))
                File.Delete(TEST_FILE_NAME);
        }

        [Test]
        public void TestMakeHttpRequest()
        {
            runner.usingDownloadedPaths = false;
            runner.pageName = "SomePageName";
            string request = runner.MakeHttpRequest();
            Assert.AreEqual("GET /SomePageName?responder=fitClient HTTP/1.1\r\n\r\n", request);

            runner.usingDownloadedPaths = true;
            request = runner.MakeHttpRequest();
            Assert.AreEqual("GET /SomePageName?responder=fitClient&includePaths=yes HTTP/1.1\r\n\r\n", request);

            runner.usingDownloadedPaths = false;
            runner.suiteFilter = "myfilter";
            request = runner.MakeHttpRequest();
            Assert.AreEqual("GET /SomePageName?responder=fitClient&suiteFilter=myfilter HTTP/1.1\r\n\r\n", request);
        }

        [Test]
        public void TestParseArgs()
        {
            bool result = runner.ParseArgs(configuration, new string[] {});
            Assert.IsFalse(result);

            result = runner.ParseArgs(configuration, new string[] {"localhost", "8081", "SomeTestPage"});
            Assert.IsTrue(result);
            Assert.AreEqual("localhost", runner.host);
            Assert.AreEqual(8081, runner.port);
            Assert.AreEqual("SomeTestPage", runner.pageName);
            Assert.AreEqual(runner.resultWriter.GetType(), typeof(NullResultWriter));
        }

        [Test]
        public void ExtraAssemblyArgs()
        {
            bool result = runner.ParseArgs(configuration, new string[] {"host", "80", "SomePage", "fit.dll", "fit.config", "testTarget.dll"});
            Assert.IsTrue(result);
            Assert.AreEqual("host", runner.host);
            Assert.AreEqual(80, runner.port);
            Assert.AreEqual("SomePage", runner.pageName);
            //todo: use a mock so we can test this
            //Assert.IsTrue(Configuration.Instance.Assemblies.HasValue("fit.dll"));
            //Assert.IsTrue(Configuration.Instance.Assemblies.HasValue("other.dll"));
            Assert.AreEqual("fit.config", Path.GetFileName(AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString()));
            Assert.AreEqual(runner.resultWriter.GetType(), typeof(NullResultWriter));
        }

        [Test]
        public void TestParseArgsWithOptions()
        {
            bool result = runner.ParseArgs(configuration, new string[] {"-v", "-debug", "-nopaths", "-suiteFilter", "myfilter", 
                "-results", "stdout", "-format", "text", "localhost", "8081", "SomeTestPage"});
            Assert.IsTrue(runner.verbose);
            Assert.IsTrue(runner.debug);
            Assert.IsFalse(runner.usingDownloadedPaths);
            Assert.IsTrue(result);
            Assert.AreEqual("localhost", runner.host);
            Assert.AreEqual(8081, runner.port);
            Assert.AreEqual(runner.resultWriter.GetType(), typeof(TextResultWriter));
            Assert.AreEqual("SomeTestPage", runner.pageName);
            //Assert.AreEqual("stdout", runner.cacheFilename);
            Assert.AreEqual("myfilter", runner.suiteFilter);
        }

        [Test]
        public void TestParseArgsWithResultOption_StandardOut()
        {
            bool result = runner.ParseArgs(configuration, new string[] {"-results", "stdout", "-format", "text", "localhost", "8081", "SomeTestPage"});
            Assert.IsFalse(runner.verbose);
            Assert.IsFalse(runner.debug);
            Assert.IsTrue(runner.usingDownloadedPaths);
            Assert.IsTrue(result);
            Assert.AreEqual("localhost", runner.host);
            Assert.AreEqual(8081, runner.port);
            Assert.AreEqual(runner.resultWriter.GetType(), typeof(TextResultWriter));
            Assert.AreEqual("SomeTestPage", runner.pageName);
        }

        [Test]
        public void TestParseArgsWithResultOption_Filename()
        {
            bool result = runner.ParseArgs(configuration, new string[] { "-results", TEST_FILE_NAME, "-format", "xml", "localhost", "8081", "SomeTestPage" });
            Assert.IsFalse(runner.verbose);
            Assert.IsFalse(runner.debug);
            Assert.IsTrue(runner.usingDownloadedPaths);
            Assert.IsTrue(result);
            Assert.AreEqual("localhost", runner.host);
            Assert.AreEqual(8081, runner.port);
            Assert.AreEqual(runner.resultWriter.GetType(), typeof(XmlResultWriter));
            Assert.AreEqual("SomeTestPage", runner.pageName);
        }

        [Test]
        public void TestVerbose()
        {
            StringWriter output = new StringWriter();
            runner.verbose = false;
            runner.output = output;

            runner.HandleFinalCount(TestUtils.MakeTestStatus());
            Assert.AreEqual("", output.ToString());
			
            runner.verbose = true;
            runner.HandleFinalCount(TestUtils.MakeTestStatus());
            string expected = "\r\n" +
                              "Test Pages: 0 right, 0 wrong, 0 ignored, 0 exceptions\r\n" +
                              "Assertions: 1 right, 2 wrong, 3 ignored, 4 exceptions\r\n";
            Assert.AreEqual(expected, output.ToString());
        }
    }
}