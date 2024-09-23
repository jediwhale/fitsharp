// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Service;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class DocumentTest {

        [Test] public void ParsesBracketsAsList() {
            fitSharp.Slim.Service.Document document = ParseAsList("[000002:000004:some:000005:stuff]", 2);
            ClassicAssert.AreEqual("some",document.Content.ValueAt(0));
            ClassicAssert.AreEqual("stuff",document.Content.ValueAt(1));
        }

        static fitSharp.Slim.Service.Document ParseAsList(string input, int expectedCount) {
            fitSharp.Slim.Service.Document document = fitSharp.Slim.Service.Document.Parse(input);
            ClassicAssert.AreEqual(expectedCount, document.Content.Branches.Count);
            return document;
        }

        [Test] public void ParsesEmptyList() {
            ParseAsList("[000000:]", 0);
        }

        [Test] public void ParsesBracketsWithoutCountAsString() {
            ParsesAsString("[some,stuff]");
        }

        static void ParsesAsString(string input) {
            fitSharp.Slim.Service.Document document = fitSharp.Slim.Service.Document.Parse(input);
            ClassicAssert.IsTrue(document.Content.IsLeaf);
            ClassicAssert.AreEqual(input,document.Content.Value);
        }

        [Test] public void ParsesBracketsWithShortCountAsString() {
            ParsesAsString("[12345:A]");
        }

        [Test] public void ComposesList() {
            ClassicAssert.AreEqual(new Document(new SlimTree().AddBranches("hello", "world")).ToString(),
                "[000002:000005:hello:000005:world:]");
        }
        
        [Test] public void ComposesNestedList() {
            ClassicAssert.AreEqual(new Document(new SlimTree().AddBranch(new SlimTree().AddBranches("hello", "world"))).ToString(),
                "[000001:000035:[000002:000005:hello:000005:world:]:]");
            ClassicAssert.AreEqual(new Document(new SlimTree().AddBranch(new SlimLeaf("[000002:000005:hello:000005:world:]"))).ToString(),
                "[000001:000035:[000002:000005:hello:000005:world:]:]");
        }
    }
}
