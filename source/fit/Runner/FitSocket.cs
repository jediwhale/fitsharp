// Copyright © 2016 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.IO;

namespace fit.Runner {
    public class FitSocket {
        public FitSocket(SocketModel socket, ProgressReporter reporter) {
            socketSession = new SocketSession(socket);
            this.reporter = reporter;
            this.socket = socket;
        }

		public void EstablishConnection(string request) {
		    reporter.WriteLine("\tHTTP request: " + request);
		    Transmit(request);

		    reporter.WriteLine("Validating connection...");
		    var statusSize = ReceiveInteger();
			if (statusSize == 0) {
			    reporter.WriteLine("\t...ok\n");
			}
			else {
				var errorMessage = socketSession.Read(statusSize);
			    reporter.WriteLine("\t...failed because: " + errorMessage);
			    Console.WriteLine("An error occured while connecting to client.");
				Console.WriteLine(errorMessage);
				Environment.Exit(-1);
			}
		}

        public void SendDocument(string document) {
            socketSession.Write(Protocol.FormatDocument(document));
        }

        public void Transmit(string message) {
            socketSession.Write(message);
		}

		public string ReceiveDocument() {
			var documentLength = ReceiveInteger();
			return documentLength == 0 ? "" : socketSession.Read(documentLength);
		}

        public void SendCounts(TestCounts counts) {
	        socketSession.Write(Protocol.FormatCounts(counts));
        }

        public void Close() {
            socket.Close();
        }

		int ReceiveInteger() {
			return Convert.ToInt32(socketSession.Read(10));
		}

        readonly SocketModel socket;
        readonly SocketSession socketSession;
        readonly ProgressReporter reporter;
    }
}
