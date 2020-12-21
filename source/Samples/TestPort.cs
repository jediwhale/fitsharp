// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.IO;

namespace fitSharp.Samples {
    public class TestPort: Port {
        public bool IsOpen = true;

        public int Receive(byte[] bytes, int offset, int bytesToRead) {
            var received = 0;
            var length = lengths[0];
            for (var i = 0; i < length && i < bytesToRead; i++) {
                bytes[offset + i] = inBuffer[0];
                inBuffer.RemoveAt(0);
                received++;
            }
            if (length == received) {
                lengths.RemoveAt(0);
            }
            else {
                lengths[0] = length - received;
            }
            return received;
        }

        public void Send(byte[] bytes) {
            foreach (var b in bytes) outBuffer.Add(b);
        }

        public void Clear() {
            inBuffer.Clear();
            lengths.Clear();
            outBuffer.Clear();
        }

        public void PutBytes(ICollection<byte> bytes) {
            lengths.Add(bytes.Count);
            foreach (var b in bytes) inBuffer.Add(b);
        }

        public byte[] GetBytes() {
            return outBuffer.ToArray();
        }

        public void Close() { IsOpen = false; }

        public string Output => MessageChannel.Decode(GetBytes());

        public void AddInput(string value) {
            PutBytes(MessageChannel.Encode(value));
        }

        readonly List<byte> inBuffer = new List<byte>();
        readonly List<byte> outBuffer = new List<byte>();
        readonly List<int> lengths = new List<int>();
    }
}
