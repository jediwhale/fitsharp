// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.IO;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.IO {
    [TestFixture] public class SocketStreamTest {

        [Test] public void SimpleStringIsRead() {
            var testSocket = new TestSocket();
            var socketStream = new SocketStream(testSocket);
            testSocket.PutBytes(new byte[] {104, 101, 108, 108, 111});
            Assert.AreEqual("hello", socketStream.ReadBytes(5));
        }

        [Test] public void StringIsReadInMultipleParts() {
            var testSocket = new TestSocket();
            var socketStream = new SocketStream(testSocket);
            testSocket.PutBytes(new byte[] {104, 101, 108, 108, 111});
            testSocket.PutBytes(new byte[] {32, 119,111,114,108,100});
            Assert.AreEqual("hello world", socketStream.ReadBytes(11));
        }

        [Test] public void EncodedStringIsRead() {
            var testSocket = new TestSocket();
            var socketStream = new SocketStream(testSocket);
            testSocket.PutBytes(new byte[] {104, 226, 128, 153, 108, 108, 111});
            Assert.AreEqual("h\u2019llo", socketStream.ReadBytes(7));
        }

        [Test] public void SimpleStringIsWritten() {
            var testSocket = new TestSocket();
            var socketStream = new SocketStream(testSocket);
            socketStream.Write("hello");
            Assert.AreEqual(new byte[] {104, 101, 108, 108, 111}, testSocket.GetBytes());
        }

        [Test] public void StringIsWrittenWithLengthPrefix() {
            var testSocket = new TestSocket();
            var socketStream = new SocketStream(testSocket);
            socketStream.Write("h\u2019llo", "{0}:");
            Assert.AreEqual(new byte[] {55, 58, 104, 226, 128, 153, 108, 108, 111}, testSocket.GetBytes());
        }

    }
}
