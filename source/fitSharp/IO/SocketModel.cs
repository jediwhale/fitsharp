// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Net.Sockets;

namespace fitSharp.IO {
    public interface SocketModel {
        int Receive(byte[] buffer, int offset, int bytesToRead);
        void Send(byte[] buffer);
        void Close();
    }

    public class SocketModelImpl: SocketModel {
        private readonly Socket socket;

        public SocketModelImpl(Socket socket) {
            this.socket = socket;
        }

        public int Receive(byte[] buffer, int offset, int bytesToRead) {
            return socket.Receive(buffer, offset, bytesToRead, SocketFlags.None);
        }

        public void Send(byte[] buffer) {
            socket.Send(buffer);
        }

        public void Close() { socket.Close(); }
    }
}
