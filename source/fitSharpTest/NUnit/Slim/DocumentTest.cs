// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class DocumentTest {

        [Test] public void ListIsParsed() {
            fitSharp.Slim.Service.Document document = fitSharp.Slim.Service.Document.Parse("[000002:000004:some:000005:stuff]");
            Assert.AreEqual("some",document.Content.Branches[0].Value);
            Assert.AreEqual("stuff",document.Content.Branches[1].Value);
        }

        [Test] public void NotAListWithoutPrefixCount() {
            fitSharp.Slim.Service.Document document = fitSharp.Slim.Service.Document.Parse("[some,stuff]");
            Assert.IsTrue(document.Content.IsLeaf);
            Assert.AreEqual("[some,stuff]",document.Content.Value);
        }
    }
}
