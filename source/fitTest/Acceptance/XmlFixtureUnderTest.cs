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