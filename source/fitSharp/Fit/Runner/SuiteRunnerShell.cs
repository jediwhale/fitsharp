// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Application;
using fitSharp.Fit.Engine;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitSharp.Fit.Runner {
    public class SuiteRunnerShell {

        public SuiteRunnerShell(Memory memory, ProgressReporter reporter, Func<Memory, CellProcessor> newService) {
            SuiteRunner = new SuiteRunner(memory, reporter, newService, new FileSystemModel(memory.GetItem<Settings>().CodePageNumber));
        }

        public string Results {get { return SuiteRunner.TestCounts.Description; }}
        public SuiteRunner SuiteRunner { get; private set; }

        public int Run(IList<string> commandLineArguments, Memory memory) {
            var result = Run(memory, commandLineArguments);
            return result;
        }

        int Run(Memory memory, IList<string> arguments) {
            ParseArguments(memory, arguments);
            var fileSystem = new FileSystemModel(memory.GetItem<Settings>().CodePageNumber);
            memory.GetItem<Context>().PageSource = fileSystem;
            memory.GetItem<Context>().SuitePath = new DirectoryPath(memory.GetItem<Settings>().InputFolder);
            SuiteRunner.Run(
                CreateStoryTestFolder(memory, fileSystem),
                selectedFile);
            return SuiteRunner.TestCounts.FailCount;
        }

        void ParseArguments(Memory memory, IList<string> arguments) {
            if (arguments.Count == 0) {
                return;
            }
            var argumentParser = new ArgumentParser();
            argumentParser.AddSwitchHandler("d", () => memory.GetItem<Settings>().DryRun = true);
            argumentParser.AddArgumentHandler("i", value => memory.GetItem<Settings>().InputFolder = value);
            argumentParser.AddArgumentHandler("o", value => memory.GetItem<Settings>().OutputFolder = value);
            argumentParser.AddArgumentHandler("s", value => selectedFile = value);
            argumentParser.AddArgumentHandler("x", value => memory.GetItem<FileExclusions>().AddRange(value.Split(';')));
            argumentParser.AddArgumentHandler("t", value => memory.GetItem<Settings>().TagList = value);

            argumentParser.Parse(arguments);
            if (memory.GetItem<Settings>().InputFolder == null)
                throw new FormatException("Missing input folder");
            if (memory.GetItem<Settings>().OutputFolder == null)
                throw new FormatException("Missing output folder");
        }
    
        StoryTestFolder CreateStoryTestFolder(Memory memory, FolderModel folderModel) {
            var filters = new Filters(memory.GetItem<Settings>().TagList, memory.GetItem<FileExclusions>(), selectedFile);
            return new StoryTestFolder(memory, folderModel, filters);
        }

        string selectedFile;
    }
}
