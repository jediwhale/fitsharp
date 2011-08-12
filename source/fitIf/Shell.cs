// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fit.Service;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitIf
{
    public class Shell {
        public Shell(string[] commandLineArguments) {
            var appDomainSetup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            testDomain = AppDomain.CreateDomain("fitSharp.Machine", null, appDomainSetup);
            runner = (Runner) testDomain.CreateInstanceAndUnwrap(
                                              Assembly.GetExecutingAssembly().GetName().Name,
                                              typeof (Runner).FullName);
            runner.SetUp(commandLineArguments);
        }

        public string Run(string input) {
            return runner.Run(input);
        }

        public void Close() {
            new Action<AppDomain>(AppDomain.Unload).BeginInvoke(testDomain, null, null);
        }

        readonly AppDomain testDomain;
        readonly Runner runner;

        class Runner: MarshalByRefObject {
            public void SetUp(string[] commandLineArguments) {
                var argumentParser = new ArgumentParser();
                argumentParser.AddArgumentHandler("c", value => new SuiteConfiguration(configuration).LoadXml(new FileSystemModel().FileContent(value)));
                argumentParser.Parse(commandLineArguments);
            }

            public string Run(string input) {
                var storyTest =
                    "<style type=\"text/css\">\n" +
                    ".pass {background-color: #AAFFAA;}\n" +
                    ".fail {background-color: #FFAAAA;}\n" +
                    ".error {background-color: #FFFFAA;}\n" +
                    ".ignore {background-color: #CCCCCC;}\n" +
                    ".fit_stacktrace {font-size: 0.7em;}\n" +
                    ".fit_label {font-style: italic; color: #C08080;}\n" +
                    ".fit_grey {color: #808080;}\n" +
                    ".fit_extension {border: solid 1px grey;}\n" +
                    ".fit_table {border: solid 1px grey; border-collapse: collapse; margin: 2px 0px;}\n" +
                    "table.fit_table tr td {border: solid 1px grey; padding: 2px 2px 2px 2px;}\n" +
                    "</style>\n" +
                    "test@\n" +
                    input;
                service = new Service(configuration);
                var test = service.Compose(new StoryTestString(storyTest));
                var writer = new Writer(service);
                new ExecuteStoryTest(new Service(configuration), writer)
                    .DoTables(test);
                return writer.Result;
            }

            readonly Configuration configuration = new TypeDictionary();
            Service service;

            class Writer: StoryTestWriter {

                public Writer(CellProcessor processor) {
                    this.processor = processor;
                }

                public string Result { get; private set; }

                public void WriteTable(Tree<Cell> table) {}

                public void WriteTest(Tree<Cell> test, TestCounts counts) {
                    Result = processor.ParseTree<Cell, StoryTestString>(test).ToString();
                }

                readonly CellProcessor processor;
            }
        }
    }
}
