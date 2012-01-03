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

        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
            var now = DateTime.Now;
            myProgressReporter = reporter;
            var result = Run(memory, commandLineArguments);
            //todo: to suiterunner?
            if (!memory.GetItem<Settings>().DryRun)
                reporter.Write(string.Format("\n{0}, time: {1}\n", Results, DateTime.Now - now));
            return result;
        }

        public string Results {get { return myRunner.TestCounts.Description; }}


        ProgressReporter myProgressReporter;
        SuiteRunner myRunner;
        string selectedFile;

        int Run(Memory memory, IList<string> arguments) {
            ParseArguments(memory, arguments);
            var fileSystem = new FileSystemModel(memory.GetItem<Settings>().CodePageNumber);
            memory.GetItem<Context>().PageSource = fileSystem;
            myRunner = new SuiteRunner(memory, myProgressReporter);
            myRunner.Run(
                CreateStoryTestFolder(memory, fileSystem),
                selectedFile);
            return myRunner.TestCounts.FailCount;
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
            var storyTestFolder = new StoryTestFolder(memory, folderModel);

            string tagList = memory.GetItem<Settings>().TagList;
            if (!string.IsNullOrEmpty(tagList))
                storyTestFolder.AddPageFilter(new TagFilter(tagList));

            return storyTestFolder;
        }
    }
}
