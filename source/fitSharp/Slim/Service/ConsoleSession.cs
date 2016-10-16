// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;
using fitSharp.IO;

namespace fitSharp.Slim.Service {
    public class ConsoleSession: Session {
        public void Write(string message, string prefixFormat) {
            RestoreConsole();
            WriteToConsole(string.Format(prefixFormat, message.Length));
        }

        public void Write(string message) {
            RestoreConsole();
            WriteToConsole(message);
        }

        public string Read(int length) {
            CaptureConsole();
            return ReadFromConsole(length);
        }

        public void Close() {
            RestoreConsole();
        }

        void WriteCaptured() {
            WriteEncoded("SOUT :", captureOut);
            WriteEncoded("SERR :", captureError);
        }

        static void WriteEncoded(string prefix, StringWriter content) {
            var encodedContent = prefix + content.ToString().Replace(Environment.NewLine, Environment.NewLine + prefix);
            if (encodedContent.EndsWith(prefix)) {
                encodedContent = encodedContent.Substring(0, encodedContent.Length - prefix.Length);
            }
            Console.Error.Write(encodedContent);
        }

        static void WriteToConsole(string message) {
            Console.Write(message);
        }

        static string ReadFromConsole(int length) {
            var result = new StringBuilder(length);
            for (var i = 0; i < length; i++) {
                result.Append((char) Console.Read());
            }
            return result.ToString();
        }

        void CaptureConsole() {
            if (isCaptured) return;
            saveOut = Console.Out;
            saveError = Console.Error;
            captureOut = new StringWriter();
            captureError = new StringWriter();
            Console.SetOut(captureOut);
            Console.SetError(captureError);
            isCaptured = true;
        }

        void RestoreConsole() {
            if (!isCaptured) return;
            Console.SetOut(saveOut);
            Console.SetError(saveError);
            WriteCaptured();
            isCaptured = false;
        }

        bool isCaptured;
        TextWriter saveOut;
        TextWriter saveError;
        StringWriter captureOut;
        StringWriter captureError;
    }
}
