// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Slim.Service {
    public class ConsoleSession: Session {

        public ConsoleSession(Memory memory) {
            this.memory = memory;
            saveOut = Console.Out;
            saveError = Console.Error;
            CaptureConsole();
        }

        public void Write(string message, string prefixFormat) {
            WriteCaptured();
            WriteToConsole(string.Format(prefixFormat, message.Length) + message);
        }

        public void Write(string message) {
            WriteCaptured();
            WriteToConsole(message);
        }

        public string Read(int length) {
            return ReadFromConsole(length);
        }

        public void Close() {
            WriteCaptured();
            Console.SetOut(saveOut);
            Console.SetError(saveError);
        }

        void WriteCaptured() {
            WriteEncoded("SOUT :", captureOut);
            WriteEncoded("SERR :", captureError);
            CaptureConsole();
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

        void WriteToConsole(string message) {
            memory.GetItem<Trace>().Write("ConsoleOut", message);
            saveOut.Write(message);
        }

        string ReadFromConsole(int length) {
            var result = new StringBuilder(length);
            for (var i = 0; i < length; i++) {
                result.Append((char) Console.Read());
            }
            var message = result.ToString();
            memory.GetItem<Trace>().Write("ConsoleIn", message);
            return message;
        }

        void CaptureConsole() {
            captureOut = new StringWriter();
            captureError = new StringWriter();
            Console.SetOut(captureOut);
            Console.SetError(captureError);
        }

        readonly Memory memory;
        readonly TextWriter saveOut;
        readonly TextWriter saveError;
        StringWriter captureOut;
        StringWriter captureError;
    }
}
