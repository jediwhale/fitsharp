// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Samples;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {

    [TestFixture]
    public class MessengerTest {

        [Test]
        public void WritesVersion() {
            socket = new TestSocket();
            messenger = new Messenger(socket);
            Assert.AreEqual("Slim -- V0.4\n", socket.GetByteString());
        }

        [Test]
        public void WritesMessageWithLengthPrefix() {
            messenger.Write("hello");
            Assert.AreEqual("000005:hello", socket.GetByteString());
        }

        [Test]
        public void ReadsMessageWithLengthPrefix() {
            socket.PutByteString("000005:hello");
            Assert.AreEqual("hello", messenger.Read());
        }

        /* slow tests
        [Test]
        public void WritesLongMessageWithLengthPrefix() {
            messenger.Write(new string('a', 1000000));
            Assert.IsTrue(socket.GetByteString().StartsWith("1000000:a"));
        }

        [Test]
        public void ReadsLongMessageWithLengthPrefix() {
            socket.PutByteString("1000000:" + new string('a', 1000000));
            Assert.AreEqual(1000000, messenger.Read().Length);
        }*/

        [SetUp]
        public void SetUp() {
            socket = new TestSocket();
            messenger = new Messenger(socket);
            socket.Clear();
        }

        TestSocket socket;
        Messenger messenger;
    }
}
