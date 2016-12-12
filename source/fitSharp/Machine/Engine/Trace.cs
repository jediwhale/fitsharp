// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Trace: Copyable {

        public Copyable Copy() {
            return new Trace {writer = writer};
        }

        public void File(string fileName) {
            writer = new StreamWriter(fileName);
        }

        public void Stop() {
            writer.Close();
            writer = StreamWriter.Null;
        }

        public void Write(string label, string message) {
            writer.WriteLine("{0:yyyy-MM-dd HH:mm:ss.fffff},{1},{2}", DateTime.Now, label, message);
            writer.Flush();
        }

        StreamWriter writer = StreamWriter.Null;
    }
}
