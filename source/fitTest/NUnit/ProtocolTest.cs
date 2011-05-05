// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Runner;
using fitnesse.fitserver;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ProtocolTest
    {

        [Test]
        public void WriteIntegerToIntegerString()
        {
            Assert.AreEqual("0000000009", Protocol.FormatInteger(9));
            Assert.AreEqual("0000000123", Protocol.FormatInteger(123));
            Assert.AreEqual("0000004444", Protocol.FormatInteger(4444));
        }

        [Test]
        public void ToTransmissionDocument()
        {
            string Content = "Here is some Text of Length 30";
            string Expected = "0000000030" + Content;

            string Actual = Protocol.FormatDocument(Content);

            Assert.AreEqual(Expected, Actual);
        }

        [Test]
        public void TestMakeHttpRequest()
        {
            string request = Protocol.FormatRequest("SomePageName", false, null);
            Assert.AreEqual("GET /SomePageName?responder=fitClient HTTP/1.1\r\n\r\n", request);

            request = Protocol.FormatRequest("SomePageName", true, null);
            Assert.AreEqual("GET /SomePageName?responder=fitClient&includePaths=yes HTTP/1.1\r\n\r\n", request);

            request = Protocol.FormatRequest("SomePageName", false, "myfilter");
            Assert.AreEqual("GET /SomePageName?responder=fitClient&suiteFilter=myfilter HTTP/1.1\r\n\r\n", request);
        }

    }
}