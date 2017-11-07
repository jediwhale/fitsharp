// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application {
    public class ShellArguments {

        public ShellArguments(FileSource fileSource, Action<string> report) {
            this.fileSource = fileSource;
            this.report = report;
        }

        public int Parse(IList<string> commandLineArguments, Func<Memory, IList<string>, int> process) {
            memory = new TypeDictionary();
            extraArguments = new List<string>();
            isValidToRun = true;

            var argumentParser = new ArgumentParser();
            argumentParser.AddArgumentHandler("a", value => memory.GetItem<AppDomainSetup>().ConfigurationFile = value);
            argumentParser.AddArgumentHandler("c", LoadSuiteConfiguration);
            argumentParser.AddArgumentHandler("r", value => memory.GetItem<Settings>().Runner = value);
            argumentParser.AddArgumentHandler("f", InitializeAndAddFolders);
            argumentParser.SetUnusedHandler(value => extraArguments.Add(value));
            argumentParser.Parse(commandLineArguments);

            if (isValidToRun && string.IsNullOrEmpty(memory.GetItem<Settings>().Runner)) {
                report("Missing runner class");
                isValidToRun = false;
            }

            if (isValidToRun) return process(memory, extraArguments);

            report("\nUsage:\n\tRunner [ -r runnerClass ][ -a appConfigFile ][ -c suiteConfigFile ] ...");
            return 1;
        }

        void LoadSuiteConfiguration(string value) {
            var content = fileSource.FileContent(value);
            if (content == null) {
                report(string.Format("Unable to load suite configuration file '{0}'", value));
                isValidToRun = false;
                return;
            }
            new SuiteConfiguration(memory).LoadXml(content);
        }


        // Initializes the Assembly Loader and adds the given folder arguments.
        static void InitializeAndAddFolders(string args) {
            /* Add each folder into the Assembly Loader */
            foreach (var a in args.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                AssemblyLoadFailureHandler.AddFolder(a);
            }
        }

        readonly FileSource fileSource;
        readonly Action<string> report;

        bool isValidToRun;
        Memory memory;
        List<string> extraArguments;
    }
}
