// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Net;
using System.Net.Sockets;
using fitSharp.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Service {
    public class Messenger {
        private static readonly IdentifierName EndIdentifier = new IdentifierName("bye");

        private readonly SocketStream stream;
        private readonly SocketModel socket;
        public bool IsEnd { get; private set; }

        public static Messenger Make(int port) {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Socket socket = listener.AcceptSocket();
            listener.Stop();
            return new Messenger(new SocketModelImpl(socket));
        }

        public Messenger(SocketModel socket) {
            this.socket = socket;
            stream = new SocketStream(socket);
            stream.Write("Slim -- V0.3\n");
        }

        public string Read() {
            int messageByteLength = int.Parse(stream.ReadBytes(6));
            stream.ReadBytes(1); // skip the colon
            string message = stream.ReadBytes(messageByteLength);
            if (EndIdentifier.Matches(message)) {
                IsEnd = true;
                socket.Close();
                return null;
            }
            return message;
        }

        public void Write(string message) {
            stream.Write(message, "{0:000000}:");
        }
    }
}