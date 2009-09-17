// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Application {
    public interface ProgressReporter {
        void Write(string theMessage);
    }

    public interface Runnable {
        int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter);
    }

    public class Shell: MarshalByRefObject {
        private readonly List<string> extraArguments = new List<string>();
        private readonly ProgressReporter progressReporter;
        public Runnable Runner { get; private set; }

        public Shell() { progressReporter = new ConsoleReporter(); }
        public Shell(ProgressReporter progressReporter) { this.progressReporter = progressReporter; }

        public int Run(string[] commandLineArguments) {
            try {
                string appConfigName = LookForAppConfig(commandLineArguments);
                return appConfigName.Length == 0
                           ? RunInCurrentDomain(commandLineArguments)
                           : RunInNewDomain(appConfigName, commandLineArguments);
            }
            catch (System.Exception e) {
                progressReporter.Write(string.Format("{0}\n", e));
                return 1;
            }
        }

        private static string LookForAppConfig(string[] commandLineArguments) {
            for (int i = 0; i < commandLineArguments.Length - 1; i++) {
                if (commandLineArguments[i] == "-a") return commandLineArguments[i + 1];
            }
            return string.Empty;
        }

        private int RunInCurrentDomain(string[] commandLineArguments) {
            ParseArguments(commandLineArguments);
            if (!ValidateArguments()) {
                progressReporter.Write("\nUsage:\n\tRunner -r runnerClass [ -a appConfigFile ][ -c runnerConfigFile ] ...\n");
                return 1;
            }
            return ExecuteRunner();
        }

        private static int RunInNewDomain(string appConfigName, string[] commandLineArguments) {
            var appDomainSetup = new AppDomainSetup {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationFile = appConfigName
            };
            AppDomain newDomain = AppDomain.CreateDomain("fitSharp.Machine", null, appDomainSetup);
            int result;
            try {
                var remoteShell = (Shell) newDomain.CreateInstanceAndUnwrap(
                                              Assembly.GetExecutingAssembly().GetName().Name,
                                              typeof (Shell).FullName);
                result = remoteShell.RunInCurrentDomain(commandLineArguments);
            }
            finally {
                AppDomain.Unload(newDomain);
            }
            return result;
        }

        private void ParseArguments(string[] commandLineArguments) {
            for (int i = 0; i < commandLineArguments.Length; i++) {
                if (i < commandLineArguments.Length - 1) {
                    switch (commandLineArguments[i]) {
                        case "-c":
                            Context.Configuration.LoadFile(commandLineArguments[i + 1]);
                            break;
                        case "-a":
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

        private static void ParseRunnerArgument(string argument) {
            string[] tokens = argument.Split(',');
            Context.Configuration.GetItem<Settings>().Runner = tokens[0];
            if (tokens.Length > 1) {
                Context.Configuration.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
        }

        private bool ValidateArguments() {
            if (string.IsNullOrEmpty(Context.Configuration.GetItem<Settings>().Runner)) {
                progressReporter.Write("Missing runner class\n");
                return false;
            }
            return true;
        }

        private int ExecuteRunner() {
            Runner = new BasicProcessor().Create(Context.Configuration.GetItem<Settings>().Runner, new TreeList<string>()).GetValue<Runnable>();
            return Runner.Run(extraArguments.ToArray(), Context.Configuration, progressReporter);
        }
    }

    public class ConsoleReporter: ProgressReporter {
        public void Write(string theMessage) {
            Console.Write(theMessage);
        }
    }

}