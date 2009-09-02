// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitSharp.Slim.Service {
    public class Runner: Runnable {
        private Messenger messenger;
        private Service service;
        private string assemblyPaths;

        public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
            service = configuration.GetItem<Service>();
            service.ApplicationUnderTest = configuration.GetItem<ApplicationUnderTest>();
            ParseCommandLine(commandLineArguments);
            new Interpreter(messenger, assemblyPaths, service).ProcessInstructions();
            return 0;
        }

        private void ParseCommandLine(string[] commandLineArguments) {
            messenger = Messenger.Make(int.Parse(commandLineArguments[commandLineArguments.Length - 1]));
            if (commandLineArguments.Length > 1) {
                assemblyPaths = commandLineArguments[0];
            }
        }
    }
}