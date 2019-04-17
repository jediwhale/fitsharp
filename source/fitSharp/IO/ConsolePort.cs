// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.IO {
    public class ConsolePort: Port {

        public ConsolePort(Memory memory) {
            this.memory = memory;
            saveOut = Console.Out;
            saveError = Console.Error;
            inputStream = Console.OpenStandardInput();
            outputStream = Console.OpenStandardOutput();
            CaptureConsole();
        }
        
        public int Receive(byte[] buffer, int offset, int bytesToRead) {
            inputStream.Read(buffer, offset, bytesToRead);
            return bytesToRead;
        }

        public void Send(byte[] buffer) {
            outputStream.Write(buffer, 0, buffer.Length);
        }

        public void Close() {
            WriteCaptured();
            inputStream.Close();
            outputStream.Close();
            Console.SetOut(saveOut);
            Console.SetError(saveError);
        }
        
        void WriteCaptured() {
            WriteEncoded("SOUT :", captureOut);
            WriteEncoded("SERR :", captureError);
            CaptureConsole();
        }
        
        void CaptureConsole() {
            captureOut = new StringWriter();
            captureError = new StringWriter();
            Console.SetOut(captureOut);
            Console.SetError(captureError);
        }
        
        void WriteEncoded(string prefix, StringWriter content) {
            var contentString = content.ToString();
            if (string.IsNullOrEmpty(contentString)) return;
            var encodedContent = prefix + contentString.Replace(Environment.NewLine, Environment.NewLine + prefix);
            if (encodedContent.EndsWith(prefix)) {
                encodedContent = encodedContent.Substring(0, encodedContent.Length - prefix.Length);
            }
            memory.GetItem<Trace>().Write("ConsoleErr", encodedContent);
            saveError.Write(encodedContent);
        }
        
        readonly Memory memory;
        readonly TextWriter saveOut;
        readonly TextWriter saveError;
        readonly Stream inputStream;
        readonly Stream outputStream;
        StringWriter captureOut;
        StringWriter captureError;
    }
}