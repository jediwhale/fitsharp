// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.IO;
using fitSharp.Samples;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {

    [TestFixture]
    public class MessengerTest {

        [Test]
        public void WritesVersion() {
            Assert.AreEqual("Slim -- V0.5\n", port.Output);
        }

        [Test]
        public void WritesMessageWithLengthPrefix() {
            port.Clear();
            messenger.Write("hello");
            Assert.AreEqual("000005:hello", port.Output);
        }

        [Test]
        public void ReadsMessageWithLengthPrefix() {
            port.Input = "000005:hello";
            Assert.AreEqual("hello", messenger.Read());
        }

        [Test]
        public void AtEndWhenByeRead() {
            port.Input = "000003:bye";
            messenger.Read();
            Assert.IsTrue(messenger.IsEnd);
            Assert.IsFalse(port.IsOpen);
        }

        /* slow tests
        [Test]
        public void WritesLongMessageWithLengthPrefix() {
            session.Output = string.Empty;
            messenger.Write(new string('a', 1000000));
            Assert.IsTrue(session.Output.StartsWith("1000000:a"));
        }

        [Test]
        public void ReadsLongMessageWithLengthPrefix() {
            session.Input = "1000000:" + new string('a', 1000000);
            Assert.AreEqual(1000000, messenger.Read().Length);
        }*/

        [SetUp]
        public void SetUp() {
            port = new TestPort();
            messenger = new Messenger(new MessageChannel(port));
        }

        TestPort port;
        Messenger messenger;
    }
}
