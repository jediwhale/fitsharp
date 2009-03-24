using System;
using System.Collections.Generic;
using System.Net.Sockets;
using fitnesse.fitserver;

namespace fit.Test.FitUnit {
    public class SocketAdapter: ISocketWrapper {
        private readonly List<byte[]> dataBytes = new List<byte[]>();

        public void PutBytes(byte[] dataBytePacket) {
            dataBytes.Add(dataBytePacket);
        }

        public string ReceiveStringOfLength(int stringLength) {
            return SocketUtils.ReceiveStringOfLength(this, stringLength).Replace('\u2019', '\'');
        }

        public int Receive(byte[] buffer, int offset, int size, SocketFlags flags) {
            if (dataBytes.Count == 0) return 0;
            int bytesReturned = Math.Min(dataBytes[0].Length, size);
            for (int i = 0; i < bytesReturned; i++) {
                buffer[offset + i] = dataBytes[0][i];
            }
            dataBytes.RemoveAt(0);
            return bytesReturned;
        }
    }
}