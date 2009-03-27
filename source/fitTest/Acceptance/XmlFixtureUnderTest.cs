// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Xml;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class XmlFixtureUnderTest: XmlFixture {

        public XmlFixtureUnderTest(): base(MakeDocument()) {}

        private static XmlDocument MakeDocument() {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><child1>some text</child1><child2 item1=\"value1\" item2=\"value2\">text1<leaf name=\"value\"/>text2</child2><child3/></root>");
            return document;
        }

    }
}