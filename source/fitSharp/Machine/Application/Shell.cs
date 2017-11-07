// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {

    public class Shell: MarshalByRefObject {

        public Shell(ProgressReporter progressReporter, FileSource fileSource, IList<string> commandLineArguments) {
            this.progressReporter = progressReporter;
            this.fileSource = fileSource;
            this.commandLineArguments = commandLineArguments;
        }

        public Runnable Runner { get; private set; }

        public int Run() {
            try {
                var arguments = new ShellArguments(fileSource, progressReporter.WriteLine);
                return arguments.Parse(commandLineArguments, RunInDomain);
            }
            catch (System.Exception e) {
                progressReporter.Write(string.Format("{0}\n", e));
                return 1;
            }
        }

        int RunInDomain(Memory memory, IList<string> extraArguments) {
            return !memory.HasItem<AppDomainSetup>()
                ? RunInCurrentDomain(memory, extraArguments)
                : RunInNewDomain(memory.GetItem<AppDomainSetup>());
        }

        int RunInNewDomain(AppDomainSetup appDomainSetup) {
            if (string.IsNullOrEmpty(appDomainSetup.ApplicationBase)) {
                appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            }
            var newDomain = AppDomain.CreateDomain("fitSharp.Machine", null, appDomainSetup);
            try {
                var remoteShell = (Shell) newDomain.CreateInstanceAndUnwrap(
                                              Assembly.GetExecutingAssembly().GetName().Name,
                                              typeof (Shell).FullName,
                                              false,
                                              BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public,
                                              null,
                                              new object[] { progressReporter, fileSource, commandLineArguments },
                                              null, null);
                return remoteShell.RunInCurrentDomain();
            }
            finally {
                // avoid deadlock on Unload
                new Action<AppDomain>(AppDomain.Unload).BeginInvoke(newDomain, null, null);
            }
        }

        int RunInCurrentDomain() {
            var arguments = new ShellArguments(fileSource, progressReporter.WriteLine);
            return arguments.Parse(commandLineArguments, RunInCurrentDomain);
        }

        int RunInCurrentDomain(Memory memory, IList<string> extraArguments) {
            var tokens = memory.GetItem<Settings>().Runner.Split(',');
            if (tokens.Length > 1) {
                memory.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
            Runner = new BasicProcessor().Create(tokens[0]).GetValue<Runnable>();
            ExecuteInApartment(memory, extraArguments);
            return result;
        }

        void ExecuteInApartment(Memory memory, IList<string> extraArguments) {
            var apartmentConfiguration = memory.GetItem<Settings>().ApartmentState;
            if (apartmentConfiguration != null) {
                var desiredState = (ApartmentState)Enum.Parse(typeof(ApartmentState), apartmentConfiguration);
                if (Thread.CurrentThread.GetApartmentState() != desiredState) {
                    var thread = new Thread(() => Run(memory, extraArguments));
                    thread.SetApartmentState(desiredState);
                    thread.Start();
                    thread.Join();
                    return;
                }
            }
            Run(memory, extraArguments);
        }

        void Run(Memory memory, IList<string> extraArguments) {
            result = Runner.Run(extraArguments, memory, progressReporter);
        }

        readonly ProgressReporter progressReporter;
        readonly FileSource fileSource;
        readonly IList<string> commandLineArguments;

        int result;
    }
}
