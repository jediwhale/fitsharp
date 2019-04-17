// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Net;
using System.Net.Sockets;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Service {
    public class Messenger {
        public bool IsEnd { get; private set; }

        public static Messenger Make(int port, Memory memory) {
            if (port == 1) {
                return new Messenger(new MessageChannel(new ConsolePort(memory)));
            }
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            var socket = listener.AcceptSocket();
            listener.Stop();
            return new Messenger(new MessageChannel(new SocketPort(socket)));
        }

        public Messenger(MessageChannel channel) {
            this.channel = channel;
            channel.Write("Slim -- V0.5\n");
        }

        public string Read() {
            var lengthString = string.Empty;
            while (true) {
                var lengthCharacter = channel.Read(1);
                if (lengthCharacter == ":") break;
                lengthString += lengthCharacter;
            }
            var messageByteLength = int.Parse(lengthString);
            var message = channel.Read(messageByteLength);
            if (EndIdentifier.Matches(message)) {
                IsEnd = true;
                channel.Close();
                return null;
            }
            return message;
        }

        public void Write(string message) {
            channel.Write(message, "{0:000000}:");
        }

        static readonly IdentifierName EndIdentifier = new IdentifierName("bye");

        readonly MessageChannel channel;
    }
}