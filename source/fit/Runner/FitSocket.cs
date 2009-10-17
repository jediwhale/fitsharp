// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitnesse.fitserver;
using fitSharp.Fit.Model;
using fitSharp.IO;

namespace fit.Runner {
    public class FitSocket {
        private readonly SocketModel socket;
        private readonly SocketStream socketStream;
        private readonly ProgressReporter reporter;

        public FitSocket(SocketModel socket, ProgressReporter reporter) {
            socketStream = new SocketStream(socket);
            this.reporter = reporter;
            this.socket = socket;
        }

		public void EstablishConnection(string request)
		{
		    reporter.WriteLine("\tHTTP request: " + request);
		    Transmit(request);

		    reporter.WriteLine("Validating connection...");
		    int StatusSize = ReceiveInteger();
			if (StatusSize == 0) {
			    reporter.WriteLine("\t...ok\n");
			}
			else {
				String errorMessage = socketStream.ReadBytes(StatusSize);
			    reporter.WriteLine("\t...failed because: " + errorMessage);
			    Console.WriteLine("An error occured while connecting to client.");
				Console.WriteLine(errorMessage);
				Environment.Exit(-1);
			}
		}

        public void SendDocument(string document) {
            socketStream.Write(Protocol.FormatDocument(document));
        }

        public void Transmit(string message) {
            socketStream.Write(message);
		}

		public string ReceiveDocument() {
			int documentLength = ReceiveInteger();
			if (documentLength == 0)
				return "";
		    return socketStream.ReadBytes(documentLength);
		}

        public void SendCounts(TestCounts counts) {
	        socketStream.Write(Protocol.FormatCounts(counts));
        }

        public void Close() {
            socket.Close();
        }

		private int ReceiveInteger() {
			return Convert.ToInt32(socketStream.ReadBytes(10));
		}
    }
}
