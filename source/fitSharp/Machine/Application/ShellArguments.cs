// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {
    [Serializable]
    public class ShellArguments {

        public ShellArguments(TextSource textSource, IList<string> commandLineArguments) {
            this.textSource = textSource;
            this.commandLineArguments = commandLineArguments;
        }

        public IEnumerable<string> Extras { get { return ArgumentParser.Extras(commandLineArguments, switches); } }

        public string Usage {
            get {
            return string.Format("Usage:\n\t{0} [ -r runnerClass ][ -a appConfigFile ][ -c suiteConfigFile ] ...",
                Process.GetCurrentProcess().ProcessName);
            }
        }

        public int Parse(Func<Memory, int> process, Action<string> report) {
            var memory = new TypeDictionary();
            isValidToRun = true;

            var argumentParser = new ArgumentParser();
            argumentParser.AddArgumentHandler("a", value => memory.GetItem<AppDomainSetup>().ConfigurationFile = value);
            argumentParser.AddArgumentHandler("c", value => {
                if (!textSource.Exists(value)) {
                    report(string.Format("Suite configuration file '{0}' does not exist.", value));
                    isValidToRun = false;
                }
                else {
                    new SuiteConfiguration(memory).LoadXml(textSource.Content(value));
                }
            });
            argumentParser.AddArgumentHandler("r", value => memory.GetItem<Settings>().Runner = value);
            argumentParser.AddArgumentHandler("f", InitializeAndAddFolders);
            argumentParser.Parse(commandLineArguments);

            var runner = memory.GetItem<Settings>().Runner;
            if (!string.IsNullOrEmpty(runner)) {
                var tokens = runner.Split(',');
                if (tokens.Length > 1) {
                    memory.GetItem<ApplicationUnderTest>().AddAssembly(tokens[1]);
                    memory.GetItem<Settings>().Runner = tokens[0];
                }
            }

            if (isValidToRun && string.IsNullOrEmpty(memory.GetItem<Settings>().Runner)) {
                report("Missing runner class");
                isValidToRun = false;
            }

            return isValidToRun ? process(memory) : 1;
        }

        // Initializes the Assembly Loader and adds the given folder arguments.
        static void InitializeAndAddFolders(string args) {
            /* Add each folder into the Assembly Loader */
            foreach (var a in args.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                AssemblyLoadFailureHandler.AddFolder(a);
            }
        }

        readonly TextSource textSource;
        readonly IList<string> commandLineArguments;

        bool isValidToRun;

        static readonly string[] switches = {"a", "c", "f", "r"};
    }
}
