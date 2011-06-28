// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Application;
using fitSharp.Fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit.Runner {
    public class FolderRunner: Runnable {

        public int Run(IList<string> commandLineArguments, Configuration configuration, ProgressReporter reporter) {
            DateTime now = DateTime.Now;
            myProgressReporter = reporter;
            int result = Run(configuration, commandLineArguments);
            reporter.Write(string.Format("\n{0}, time: {1}\n", Results, DateTime.Now - now));
            return result;
        }

        private int Run(Configuration configuration, IList<string> arguments) {
            ParseArguments(configuration, arguments);
            myRunner = new SuiteRunner(configuration, myProgressReporter);
            myRunner.Run(
                new StoryTestFolder(configuration, new FileSystemModel(configuration.GetItem<Settings>().CodePageNumber)),
                selectedFile);
            return myRunner.TestCounts.FailCount;
        }

        public string Results {get { return myRunner.TestCounts.Description; }}

        private void ParseArguments(Configuration configuration, IList<string> arguments) {
            if (arguments.Count == 0) {
                return;
            }
            var argumentParser = new ArgumentParser();
            argumentParser.AddArgumentHandler("i", value => configuration.GetItem<Settings>().InputFolder = value);
            argumentParser.AddArgumentHandler("o", value => configuration.GetItem<Settings>().OutputFolder = value);
            argumentParser.AddArgumentHandler("s", value => selectedFile = value);
            argumentParser.AddArgumentHandler("x", value => configuration.GetItem<FileExclusions>().AddRange(value.Split(';')));
            argumentParser.Parse(arguments);
            if (configuration.GetItem<Settings>().InputFolder == null)
                throw new FormatException("Missing input folder");
            if (configuration.GetItem<Settings>().OutputFolder == null)
                throw new FormatException("Missing output folder");
        }
    
        private ProgressReporter myProgressReporter;
        private SuiteRunner myRunner;
        string selectedFile;
    }
}
