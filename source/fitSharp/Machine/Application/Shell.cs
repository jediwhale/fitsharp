// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {
    public interface Runnable {
        int Run(string[] commandLineArguments, Configuration configuration);
    }

    public class Shell {
        private string runnerName;
        private readonly List<string> extraArguments = new List<string>();
        private readonly Configuration configuration = new Configuration();

        public int Run(string[] commandLineArguments) {
            ParseArguments(commandLineArguments);
            if (!ValidateArguments()) {
                Console.WriteLine("\nUsage:\n\tRunner -r runnerClass [ -c configFile ] ...");
                return 1;
            }
            return ExecuteRunner();
        }

        private void ParseArguments(string[] commandLineArguments) {
            for (int i = 0; i < commandLineArguments.Length; i++) {
                if (i < commandLineArguments.Length - 1) {
                    switch (commandLineArguments[i]) {
                        case "-c":
                            configuration.LoadFile(commandLineArguments[i + 1]);
                            break;
                        case "-r":
                            ParseRunnerArgument(commandLineArguments[i + 1]);
                            break;
                        default:
                            extraArguments.Add(commandLineArguments[i]);
                            continue;
                    }
                    i++;
                }
                else extraArguments.Add(commandLineArguments[i]);
            }
        }

        private void ParseRunnerArgument(string argument) {
            string[] tokens = argument.Split(',');
            runnerName = tokens[0];
            if (tokens.Length > 1) {
                configuration.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
        }

        private bool ValidateArguments() {
            if (string.IsNullOrEmpty(runnerName)) {
                Console.WriteLine("Missing runner class");
                return false;
            }
            return true;
        }

        private int ExecuteRunner() {
            var runnable = (Runnable) new BasicProcessor().Create(runnerName).Value;
            return runnable.Run(extraArguments.ToArray(), configuration);
        }
    }
}