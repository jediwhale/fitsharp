// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
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
            int result = Run(commandLineArguments);
            reporter.Write(string.Format("\n{0}, time: {1}\n", Results, DateTime.Now - now));
            return result;
        }

        private int Run(string[] theArguments) {
            ParseArguments(theArguments);
            myRunner = new SuiteRunner(myProgressReporter);
            myRunner.Run(new StoryTestFolder(new FileSystemModel()), string.Empty);
            return myRunner.TestStatus.FailCount;
        }

        public string Results {get { return myRunner.TestStatus.CountDescription; }}

        private static void ParseArguments(string[] theArguments) {
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
                            Context.Configuration.GetItem<Settings>().InputFolder = argument;
                            break;
                        case "o":
                            //Configuration.Instance.Settings.OutputFolder = argument;
                            break;
                        case "x":
                            foreach (string pattern in argument.Split(';')) {
                                Context.Configuration.GetItem<FileExclusions>().Add(pattern);
                            }
                            break;
                        default:
                            //throw new FormatException("Invalid switch");
                            break;
                    }
                }
            }
            if (Context.Configuration.GetItem<Settings>().InputFolder == null)
                throw new FormatException("Missing input folder");
            if (Context.Configuration.GetItem<Settings>().OutputFolder == null)
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