// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;

namespace fitSharp.IO {
    public class MessageChannel {
	    public static byte[] Encode(string message) {
		    return Encoding.UTF8.GetBytes(message);
	    }

	    public static string Decode(byte[] bytes) {
		    var characters = new char[bytes.Length];
		    var charCount = Encoding.UTF8.GetDecoder().GetChars(bytes, 0, bytes.Length, characters, 0);
		    return new StringBuilder(charCount).Append(characters, 0, charCount).ToString();
	    }
	    
        public MessageChannel(Port port) {
            this.port = port;
        }

        public string Read(int bytesToRead) {
            var bytes = new byte[bytesToRead];
			var bytesReceived = 0;
            while (bytesReceived < bytesToRead) {
			    bytesReceived += port.Receive(bytes, bytesReceived, bytesToRead - bytesReceived);
            }

            return Decode(bytes);
        }

        public void Write(string message) {
			port.Send(Encode(message));
        }

        public void Write(string message, string prefixFormat) {
			var messageBytes = Encode(message);
            Write(string.Format(prefixFormat, messageBytes.Length));
			port.Send(messageBytes);
        }

        public void Close() {
            port.Close();
        }

        readonly Port port;
    }
}
