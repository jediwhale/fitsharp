// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {

    public class Shell: MarshalByRefObject {
        public Runnable Runner { get; private set; }

        public Shell() {
            progressReporter = new ConsoleReporter();
            folderModel = new FileSystemModel();
        }

        public Shell(ProgressReporter progressReporter, FolderModel folderModel) {
            this.progressReporter = progressReporter;
            this.folderModel = folderModel;
        }

        public int Run(IList<string> commandLineArguments) {
            try {
                ParseArguments(commandLineArguments);
                return !configuration.HasItem<AppDomainSetup>()
                           ? RunInCurrentDomain()
                           : RunInNewDomain(configuration.GetItem<AppDomainSetup>(), commandLineArguments);
            }
            catch (System.Exception e) {
                progressReporter.Write(string.Format("{0}\n", e));
                return 1;
            }
        }

        int RunInCurrentDomain(IList<string> commandLineArguments) {
            ParseArguments(commandLineArguments);
            return RunInCurrentDomain();
        }

        int RunInCurrentDomain() {
            if (!ValidateArguments()) {
                progressReporter.Write("\nUsage:\n\tRunner -r runnerClass [ -a appConfigFile ][ -c runnerConfigFile ] ...\n");
                return 1;
            }
            return Execute();
        }

        static int RunInNewDomain(AppDomainSetup appDomainSetup, IList<string> commandLineArguments) {
            if (string.IsNullOrEmpty(appDomainSetup.ApplicationBase)) {
                appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            }
            var newDomain = AppDomain.CreateDomain("fitSharp.Machine", null, appDomainSetup);
            int result;
            try {
                var remoteShell = (Shell) newDomain.CreateInstanceAndUnwrap(
                                              Assembly.GetExecutingAssembly().GetName().Name,
                                              typeof (Shell).FullName);
                result = remoteShell.RunInCurrentDomain(commandLineArguments);
            }
            finally {
                // avoid deadlock on Unload
                new Action<AppDomain>(AppDomain.Unload).BeginInvoke(newDomain, null, null);
            }
            return result;
        }

        void ParseArguments(IList<string> commandLineArguments) {
            var argumentParser = new ArgumentParser();
            argumentParser.AddArgumentHandler("a", value => configuration.GetItem<AppDomainSetup>().ConfigurationFile = Path.GetFullPath(value));
            argumentParser.AddArgumentHandler("c", value => new SuiteConfiguration(configuration).LoadXml(folderModel.FileContent(value)));
            argumentParser.AddArgumentHandler("r", value => configuration.GetItem<Settings>().Runner = value);
            argumentParser.SetUnusedHandler(value => extraArguments.Add(value));
            argumentParser.Parse(commandLineArguments);

        }

        bool ValidateArguments() {
            if (string.IsNullOrEmpty(configuration.GetItem<Settings>().Runner)) {
                progressReporter.Write("Missing runner class\n");
                return false;
            }
            return true;
        }

        private int Execute() {
            var tokens = configuration.GetItem<Settings>().Runner.Split(',');
            if (tokens.Length > 1) {
                configuration.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
            Runner = new BasicProcessor().Create(tokens[0]).GetValue<Runnable>();
            ExecuteInApartment();
            return result;
        }

        private void ExecuteInApartment() {
            var apartmentConfiguration = configuration.GetItem<Settings>().ApartmentState;
            if (apartmentConfiguration != null) {
                var desiredState = (ApartmentState)Enum.Parse(typeof(ApartmentState), apartmentConfiguration);
                if (Thread.CurrentThread.GetApartmentState() != desiredState) {
                    var thread = new Thread(Run);
                    thread.SetApartmentState(desiredState);
                    thread.Start();
                    thread.Join();
                    return;
                }
            }
            Run();
        }

        private void Run() {
            result = Runner.Run(extraArguments.ToArray(), configuration, progressReporter);
        }

        readonly List<string> extraArguments = new List<string>();
        readonly ProgressReporter progressReporter;
        readonly FolderModel folderModel;
        readonly Configuration configuration = new TypeDictionary();

        int result;
    }
}
