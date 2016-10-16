// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Net;
using System.Net.Sockets;
using fitSharp.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Service {
    public class Messenger {
        private readonly Session session;
        private static readonly IdentifierName EndIdentifier = new IdentifierName("bye");

        public bool IsEnd { get; private set; }

        public static Messenger Make(int port) {
            if (port == 1) {
                return new Messenger(new ConsoleSession());
            }
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            var socket = listener.AcceptSocket();
            listener.Stop();
            return new Messenger(new SocketSession(new SocketModelImpl(socket)));
        }

        public Messenger(Session session) {
            this.session = session;
            session.Write("Slim -- V0.5\n");
        }

        public string Read() {
            string lengthString = string.Empty;
            while (true) {
                var lengthCharacter = session.Read(1);
                if (lengthCharacter == ":") break;
                lengthString += lengthCharacter;
            }
            int messageByteLength = int.Parse(lengthString);
            string message = session.Read(messageByteLength);
            if (EndIdentifier.Matches(message)) {
                IsEnd = true;
                session.Close();
                return null;
            }
            return message;
        }

        public void Write(string message) {
            session.Write(message, "{0:000000}:");
        }
    }
}