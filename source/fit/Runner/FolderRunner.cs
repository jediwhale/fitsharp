// Copyright © 2009 Syterra Software Inc.
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

        public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
            DateTime now = DateTime.Now;
            myProgressReporter = reporter;
            int result = Run(configuration, commandLineArguments);

            // TODO: This reporting should really be moved into SuiteRunner, I think.            
            reporter.Write(string.Format("\n{0}, time: {1}\n", Results, DateTime.Now - now));

            return result;
        }

        private int Run(Configuration configuration, string[] commandLineArguments) {
            ParseArguments(configuration, commandLineArguments);
            myRunner = new SuiteRunner(configuration, myProgressReporter);                                   

            myRunner.Run(
                CreateStoryTestFolder(configuration),
                string.Empty);
            return myRunner.TestCounts.FailCount;
        }

        private StoryTestFolder CreateStoryTestFolder(Configuration configuration) {
            StoryTestFolder storyTestFolder = new StoryTestFolder(configuration, new FileSystemModel(configuration.GetItem<Settings>().CodePageNumber));

            string tagList = configuration.GetItem<Settings>().TagList;
            if (!string.IsNullOrEmpty(tagList))
                storyTestFolder.AddPageFilter(new TagFilter(tagList));

            return storyTestFolder;
        }

        public string Results {get { return myRunner.TestCounts.Description; }}

        private static void ParseArguments(Configuration configuration, string[] commandLineArguments) {
            ArgumentParser argumentParser = new ArgumentParser();
            argumentParser.AddArgumentHandler("i", (value) => { configuration.GetItem<Settings>().InputFolder = value;});
            argumentParser.AddArgumentHandler("o", (value) => { configuration.GetItem<Settings>().OutputFolder = value; });
            argumentParser.AddArgumentHandler("x", (value) => {
                foreach (string pattern in value.Split(';')) {
                    configuration.GetItem<FileExclusions>().Add(pattern);
                }
            }
            );
            argumentParser.AddArgumentHandler("t", (value) => { configuration.GetItem<Settings>().TagList = value; });

            argumentParser.Parse(commandLineArguments);

            if (configuration.GetItem<Settings>().InputFolder == null) {
                throw new FormatException("Missing input folder");
            }
            if (configuration.GetItem<Settings>().OutputFolder == null) {
                throw new FormatException("Missing output folder");
            }
        }
	    
        private ProgressReporter myProgressReporter;
        private SuiteRunner myRunner;
    }
}