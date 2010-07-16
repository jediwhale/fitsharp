// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
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
        readonly FolderModel folderModel;
        readonly Configuration configuration = new Configuration();
        string appConfigArgument;
        public Runnable Runner { get; private set; }

        public Shell() {
            progressReporter = new ConsoleReporter();
            folderModel = new FileSystemModel();
        }

        public Shell(ProgressReporter progressReporter, FolderModel folderModel) {
            this.progressReporter = progressReporter;
            this.folderModel = folderModel;
        }

        public int Run(string[] commandLineArguments) {
            try {
                ParseArguments(commandLineArguments);
                string appConfigName = LookForAppConfig();
                return appConfigName.Length == 0
                           ? RunInCurrentDomain()
                           : RunInNewDomain(appConfigName, commandLineArguments);
            }
            catch (System.Exception e) {
                progressReporter.Write(string.Format("{0}\n", e));
                return 1;
            }
        }

        string LookForAppConfig() {
            if (!string.IsNullOrEmpty(appConfigArgument)) return Path.GetFullPath(appConfigArgument);
            string appConfigSettings = configuration.GetItem<Settings>().AppConfigFile;
            if (!string.IsNullOrEmpty(appConfigSettings)) return appConfigSettings;
            return string.Empty;
        }

        int RunInCurrentDomain(string[] commandLineArguments) {
            ParseArguments(commandLineArguments);
            return RunInCurrentDomain();
        }

        int RunInCurrentDomain() {
            if (!ValidateArguments()) {
                progressReporter.Write("\nUsage:\n\tRunner -r runnerClass [ -a appConfigFile ][ -c runnerConfigFile ] ...\n");
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
                            configuration.LoadXml(folderModel.FileContent(commandLineArguments[i + 1]));
                            break;
                        case "-a":
                            appConfigArgument = commandLineArguments[i + 1];
                            break;
                        case "-r":
                            configuration.GetItem<Settings>().Runner = commandLineArguments[i + 1];
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


        bool ValidateArguments() {
            if (string.IsNullOrEmpty(configuration.GetItem<Settings>().Runner)) {
                progressReporter.Write("Missing runner class\n");
                return false;
            }
            return true;
        }

        int ExecuteRunner() {
            string[] tokens = configuration.GetItem<Settings>().Runner.Split(',');
            if (tokens.Length > 1) {
                configuration.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
            Runner = new BasicProcessor().Create(tokens[0]).GetValue<Runnable>();
            return Runner.Run(extraArguments.ToArray(), configuration, progressReporter);
        }
    }
}
