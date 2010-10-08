// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Threading;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application
{
    public class Runner {
        private readonly string[] arguments;
        private readonly Configuration configuration;
        private readonly ProgressReporter progressReporter;
        private int result;

        public Runnable Runnable { get; private set; }

        public Runner(string[] arguments, Configuration configuration, ProgressReporter progressReporter) {
            this.arguments = arguments;
            this.configuration = configuration;
            this.progressReporter = progressReporter;
        }

        public int Execute() {
            string[] tokens = configuration.GetItem<Settings>().Runner.Split(',');
            if (tokens.Length > 1) {
                configuration.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
            }
            Runnable = new BasicProcessor().Create(tokens[0]).GetValue<Runnable>();
            ExecuteInApartment();
            return result;
        }

        private void ExecuteInApartment() {
            string apartmentConfiguration = configuration.GetItem<Settings>().ApartmentState;
            if (apartmentConfiguration != null) {
                var desiredState = (ApartmentState)Enum.Parse(typeof(ApartmentState), apartmentConfiguration);
                if (Thread.CurrentThread.GetApartmentState() != desiredState) {
                    var thread = new Thread(Run);
                    thread.SetApartmentState(desiredState);
                    thread.Start(configuration);
                    thread.Join();
                    return;
                }
            }
            Run();
        }

        private void Run() {
            result = Runnable.Run(arguments, configuration, progressReporter);
        }
    }
}
