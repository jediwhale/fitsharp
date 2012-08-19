// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Text;
using fitSharp.IO;

namespace fitSharp.Samples {
    public class TestSocket: SocketModel {
        public bool isOpen = true;
        private readonly List<byte> buffer = new List<byte>();
        private readonly List<int> lengths = new List<int>();

        public int Receive(byte[] bytes, int offset, int bytesToRead) {
            var length = lengths[0];
            for (int i = 0; i < length; i++) {
                bytes[offset + i] = buffer[0];
                buffer.RemoveAt(0);
            }
            lengths.RemoveAt(0);
            return length;
        }

        public void Send(byte[] buffer) {
            PutBytes(buffer);
        }

        public void PutBytes(ICollection<byte> bytes) {
            lengths.Add(bytes.Count);
            foreach (var b in bytes) buffer.Add(b);
        }

        public void PutByteString(string bytes) {
			PutBytes(Encoding.UTF8.GetBytes(bytes));
        }

        public byte[] GetBytes() {
            return buffer.ToArray();
        }

        public string GetByteString() {
            var characters = new char[GetBytes().Length];
            int length = Encoding.UTF8.GetDecoder().GetChars(GetBytes(), 0, GetBytes().Length, characters, 0);
			return new StringBuilder(length).Append(characters, 0, length).ToString();
        }

        public void Close() { isOpen = false; }
    }
}
