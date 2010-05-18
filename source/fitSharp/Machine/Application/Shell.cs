// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {
    public interface Runnable {
        int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter);
    }

    public class Shell: MarshalByRefObject {
        readonly List<string> extraArguments = new List<string>();
        readonly ProgressReporter progressReporter;
        readonly Configuration configuration = new Configuration();
        public Runnable Runner { get; private set; }

        public Shell() { progressReporter = new ConsoleReporter(); }
        public Shell(ProgressReporter progressReporter) { this.progressReporter = progressReporter; }

        public int Run(string[] commandLineArguments) {
#if DEBUG
            if (FindDebugArg(commandLineArguments))
                System.Diagnostics.Debugger.Break();
#endif
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

        private bool FindDebugArg(string[] commandLineArguments)
        {
            for (int i = 0; i < commandLineArguments.Length; i++)
            {
                string arg = commandLineArguments[i];
                if (arg != null && arg.ToLower().Equals("-debug"))
                    return true;
            }
            return false;
        }

        static string LookForAppConfig(string[] commandLineArguments)
        {
            for (int i = 0; i < commandLineArguments.Length - 1; i++) {
                if (commandLineArguments[i] == "-a") return commandLineArguments[i + 1];
            }
            return string.Empty;
        }

        int RunInCurrentDomain(string[] commandLineArguments) {
            ParseArguments(commandLineArguments);
            if (!ValidateArguments()) {
#if DEBUG
                progressReporter.Write("\nUsage:\n\tRunner -r runnerClass [-debug] [ -a appConfigFile ][ -c runnerConfigFile ] ...\n");
#else
                progressReporter.Write("\nUsage:\n\tRunner -r runnerClass [ -a appConfigFile ][ -c runnerConfigFile ] ...\n");
#endif
                return 1;
            }
            return ExecuteRunner();
        }

        static int RunInNewDomain(string appConfigName, string[] commandLineArguments) {
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

        void ParseArguments(string[] commandLineArguments) {
            for (int i = 0; i < commandLineArguments.Length; i++) {
                if (i < commandLineArguments.Length - 1) {
                    switch (commandLineArguments[i]) {
                        case "-c":
                            configuration.LoadFile(commandLineArguments[i + 1]);
                            break;
                        case "-a":
                            break;
                        case "-r":
                            ParseRunnerArgument(commandLineArguments[i + 1]);
                            break;
                        case "-debug":
                            continue;
                        default:
                            extraArguments.Add(commandLineArguments[i]);
                            continue;
                    }
                    i++;
                }
                else extraArguments.Add(commandLineArguments[i]);
            }
        }

        void ParseRunnerArgument(string argument) {
            string[] tokens = argument.Split(',');
            configuration.GetItem<Settings>().Runner = tokens[0];
            if (tokens.Length > 1) {
                configuration.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
        }

        bool ValidateArguments() {
            if (string.IsNullOrEmpty(configuration.GetItem<Settings>().Runner)) {
                progressReporter.Write("Missing runner class\n");
                return false;
            }
            return true;
        }

        int ExecuteRunner() {
            Runner = new BasicProcessor().Create(configuration.GetItem<Settings>().Runner).GetValue<Runnable>();
            return Runner.Run(extraArguments.ToArray(), configuration, progressReporter);
        }
    }
}