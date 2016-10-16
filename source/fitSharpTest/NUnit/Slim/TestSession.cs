// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.IO;

namespace fitSharp.Test.NUnit.Slim {
    public class TestSession: Session {
        public string Output;
        public string Input;
        public bool IsClosed;

        public void Write(string message, string prefixFormat) {
            Write(string.Format(prefixFormat, message.Length));
            Write(message);
        }

        public void Write(string message) {
            Output += message;
        }

        public string Read(int length) {
            if (Input.Length == 0) Input = "000003:bye";
            var result = Input.Substring(0, length);
            Input = Input.Substring(length);
            return result;
        }

        public void Close() {
            IsClosed = true;
        }
    }
}
