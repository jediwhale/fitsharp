// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {

    public class Shell: MarshalByRefObject {

        public Shell(ProgressReporter progressReporter, ShellArguments arguments) {
            this.progressReporter = progressReporter;
            this.arguments = arguments;
        }

        public Runnable Runner { get; private set; }

        public int Run() {
            try {
                return arguments.LoadMemory().Select(ReportError, RunInDomain);
            }
            catch (System.Exception e) {
                progressReporter.WriteLine(e.ToString());
                return 1;
            }
        }

        int ReportError(string errorText) {
            progressReporter.WriteLine(errorText);
            progressReporter.WriteLine(arguments.Usage);
            return 1;
        }

        int RunInDomain(Memory memory) {
            return !memory.HasItem<AppDomainSetup>()
                ? RunInCurrentDomain(memory)
                : RunInNewDomain(memory.GetItem<AppDomainSetup>());
        }
        
        int RunInNewDomain(AppDomainSetup appDomainSetup) {
            var newDomain = AppDomain.CreateDomain("fitSharp.Machine", null, appDomainSetup);
            try {
                var remoteShell = (Shell) newDomain.CreateInstanceAndUnwrap(
                                              Assembly.GetExecutingAssembly().GetName().Name,
                                              typeof (Shell).FullName,
                                              false,
                                              BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public,
                                              null,
                                              new object[] { progressReporter, arguments },
                                              null, null);
                return remoteShell.RunInNewDomain();
            }
            finally {
                // avoid deadlock on Unload
                new Action<AppDomain>(AppDomain.Unload).BeginInvoke(newDomain, null, null);
            }
        }

        int RunInNewDomain() {
            return arguments.LoadMemory().Select(ReportError, RunInCurrentDomain);
        }

        int RunInCurrentDomain(Memory memory) {
            Runner = new BasicProcessor().Create(memory.GetItem<Settings>().Runner).GetValue<Runnable>();
            ExecuteInApartment(memory);
            return result;
        }

        void ExecuteInApartment(Memory memory) {
            var apartmentConfiguration = memory.GetItem<Settings>().ApartmentState;
            if (apartmentConfiguration != null) {
                var desiredState = (ApartmentState)Enum.Parse(typeof(ApartmentState), apartmentConfiguration);
                if (Thread.CurrentThread.GetApartmentState() != desiredState) {
                    var thread = new Thread(() => Run(memory));
                    thread.SetApartmentState(desiredState);
                    thread.Start();
                    thread.Join();
                    return;
                }
            }
            Run(memory);
        }

        void Run(Memory memory) {
            result = Runner.Run(arguments.Extras.ToList(), memory, progressReporter);
        }

        readonly ProgressReporter progressReporter;
        readonly ShellArguments arguments;

        int result;
    }
}
