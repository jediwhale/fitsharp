// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Service {
    class Messenger {
        private static readonly IdentifierName EndIdentifier = new IdentifierName("bye");
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        public bool IsEnd { get; private set; }

        public static Messenger Make(int port) {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Socket socket = listener.AcceptSocket();
            listener.Stop();
            return new Messenger(new NetworkStream(socket));
        }

        public Messenger(Stream stream) {
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.Write("Slim -- V0.0\n");
            writer.Flush();
        }

        public string Read() {
            int messageLength = int.Parse(Read(6));
            Read(1); // skip the colon
            string message = Read(messageLength);
            IsEnd = EndIdentifier.Matches(message);
            return IsEnd ? null : message;
        }

        public void Write(string message) {
            writer.Write(string.Format("{0:000000}:", message.Length));
            writer.Write(message);
            writer.Flush();
        }

        private string Read(int characterLength) {
            var message = new StringBuilder(characterLength);
            int charactersRemaining = characterLength;
            while (charactersRemaining > 0) {
                var messageCharacters = new char[charactersRemaining];
                int charactersUsed = reader.Read(messageCharacters, 0, charactersRemaining);
                message.Append(messageCharacters, 0, charactersUsed);
                charactersRemaining -= charactersUsed;
            }
            return message.ToString();
        }
    }
}