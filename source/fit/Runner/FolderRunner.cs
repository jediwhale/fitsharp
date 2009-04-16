// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Application;
using fitSharp.Machine.Application;

namespace fit.Runner {
    public class FolderRunner: Runnable {

        public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
            DateTime now = DateTime.Now;
            myProgressReporter = reporter;
            int result = Run(configuration, commandLineArguments);
            reporter.Write(string.Format("\n{0}, time: {1}\n", Results, DateTime.Now - now));
            return result;
        }

        private int Run(Configuration configuration, string[] theArguments) {
            ParseArguments(configuration, theArguments);
            myRunner = new SuiteRunner(configuration, myProgressReporter);
            myRunner.Run(new StoryTestFolder(configuration, new FileSystemModel()), string.Empty);
            return myRunner.TestStatus.FailCount;
        }

        public string Results {get { return myRunner.TestStatus.CountDescription; }}

        private static void ParseArguments(Configuration configuration, string[] theArguments) {
            if (theArguments.Length == 0) {
                return;
            }
            string lastSwitch = string.Empty;
            foreach (string argument in theArguments) {
                if (argument.StartsWith("-")) {
                    lastSwitch = argument.Substring(1).ToLower();
                }
                else {
                    switch (lastSwitch) {
                        case "a":
                            //LoadAssemblies(argument);
                            break;
                        case "c":
                            //Configuration.Instance.LoadFile(argument);
                            break;
                        case "i":
                            configuration.GetItem<Settings>().InputFolder = argument;
                            break;
                        case "o":
                            //Configuration.Instance.Settings.OutputFolder = argument;
                            break;
                        case "x":
                            foreach (string pattern in argument.Split(';')) {
                                configuration.GetItem<FileExclusions>().Add(pattern);
                            }
                            break;
                        default:
                            //throw new FormatException("Invalid switch");
                            break;
                    }
                }
            }
            if (configuration.GetItem<Settings>().InputFolder == null)
                throw new FormatException("Missing input folder");
            if (configuration.GetItem<Settings>().OutputFolder == null)
                throw new FormatException("Missing output folder");
        }

        //private static void LoadAssemblies(string theAssemblyList) {
        //    foreach (string assemblyName in theAssemblyList.Split(';')) {
        //        //Configuration.Instance.Assemblies.Add(assemblyName);
        //        Configuration.Instance.ApplicationUnderTest.AddAssembly(assemblyName);
        //    }
        //}
	    
        private ProgressReporter myProgressReporter;
        private SuiteRunner myRunner;
    }
}