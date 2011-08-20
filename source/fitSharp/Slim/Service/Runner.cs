// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitSharp.Slim.Service {
    public class Runner: Runnable {
        private Messenger messenger;
        private Service service;
        private string assemblyPaths;

        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
            service = new Service(memory);
            ParseCommandLine(commandLineArguments);
            new Interpreter(messenger, assemblyPaths, service).ProcessInstructions();
            return 0;
        }

        private void ParseCommandLine(IList<string> commandLineArguments) {
            messenger = Messenger.Make(int.Parse(commandLineArguments[commandLineArguments.Count - 1]));
            if (commandLineArguments.Count > 1) {
                assemblyPaths = commandLineArguments[0];
            }
        }
    }
}