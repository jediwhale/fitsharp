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
#if DEBUG
            if (FindDebugArg(commandLineArguments))
                System.Diagnostics.Debugger.Break();
#endif
            try {
                ParseArguments(commandLineArguments);
                string appConfigName = LookForAppConfig();
                return appConfigName.Length == 0
                           ? RunInCurrentDomain()
                           : RunInNewDomain(appConfigName, commandLineArguments);
                string domainSetupFile = LookForSwitchValue("-d", commandLineArguments);
                string appConfigName = LookForSwitchValue("-a", commandLineArguments);
                if (domainSetupFile.Length > 0) {
                    AppDomainParameters parms = AppDomainParameters.Read(domainSetupFile);
                    return RunInNewDomain(parms, commandLineArguments);
                } else if (appConfigName.Length > 0) {
                    return RunInNewDomain(appConfigName, commandLineArguments);
                } else {
                    return RunInCurrentDomain(commandLineArguments);
                }
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

        static string LookForSwitchValue(string switchName, string[] commandLineArguments)
        {
            for (int i = 0; i < commandLineArguments.Length - 1; i++) {
                if (commandLineArguments[i] == switchName) return commandLineArguments[i + 1];
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
                progressReporter.WriteLine("Usage:");
#if DEBUG
                progressReporter.WriteLine("\tRunner -r runnerClass [-debug] [ -d domainSetupFile ][ -a appConfigFile ][ -c runnerConfigFile ] ...\n");
                progressReporter.WriteLine("\t-debug\tcauses you to be prompted to attach with a debugger before the runner does anything");
#else
                progressReporter.WriteLine("\tRunner -r runnerClass [ -d domainSetupFile ][ -a appConfigFile ][ -c runnerConfigFile ][ -d domainSetupFile ] ...\n");
#endif
                progressReporter.WriteLine("\t\r\tclass used to run tests.  this will usually be either 'fitnesse.fitserver.FitServer,fit.dll' or 'fitSharp.Slim.Service.Runner,fitSharp.dll'");
                progressReporter.WriteLine("\t-d\tallows you to specify most settings used to create the AppDomain your tests will run in.  useful if you are having trouble getting the runner to load assemblies referenced by your fixtures or if you need to specify other domain setup items.  see fitSharp.Machine.Application.AppDomainParameters for more info.");
                progressReporter.WriteLine("\t-a\tallows you to specify the application config file used when running tests.  ignored if -d is given");
                progressReporter.WriteLine("\t-c\tallows you to specify a runner specific configuration file.  this is not the same as the .NET framework config file, but is passed as a parameter to the runner class specified with -r");
                return 1;
            }
            return ExecuteRunner();
        }

        static int RunInNewDomain(string appConfigName, string[] commandLineArguments) {
            var appDomainSetup = new AppDomainSetup {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationFile = appConfigName
            };
            return RunInNewDomain(appDomainSetup, commandLineArguments);
        }

        static int RunInNewDomain(AppDomainParameters parms, string[] commandLineArguments) {
            var appDomainSetup = new AppDomainSetup();
            parms.CopyTo(appDomainSetup);
            return RunInNewDomain(appDomainSetup, commandLineArguments);
        }

        private static int RunInNewDomain(AppDomainSetup appDomainSetup, string[] commandLineArguments) {
            int result;
            AppDomain newDomain = AppDomain.CreateDomain("fitSharp.Machine", null, appDomainSetup);
            try {
                var remoteShell = (Shell)newDomain.CreateInstanceAndUnwrap(
                                              Assembly.GetExecutingAssembly().GetName().Name,
                                              typeof(Shell).FullName);
                result = remoteShell.RunInCurrentDomain(commandLineArguments);
            } finally {
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
                        case "-d":
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