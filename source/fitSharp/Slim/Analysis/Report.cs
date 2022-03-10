// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Service;

namespace fitSharp.Slim.Analysis {
    public class Report : Runnable {
        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
			RunReport(memory.GetItem<ApplicationUnderTest>(),
                new StreamReader(memory.GetItem<Settings>().Input),
                new StreamWriter(memory.GetItem<Settings>().Output));
            return 0;
        }

        public static void RunReport(ApplicationUnderTest applicationUnderTest, TextReader reader, TextWriter writer) {
            new ReportInstance(applicationUnderTest, reader, writer).Run();
            writer.Close();
        }
        
        class ReportInstance {
            public ReportInstance(ApplicationUnderTest applicationUnderTest, TextReader reader, TextWriter writer) {
                this.applicationUnderTest = applicationUnderTest;
                this.reader = reader;
                this.writer = writer;
            }

            public void Run() {
                new Break<string, string>(StartPage, Detail, EndPage).Process(Lines());
            }

            void StartPage(string pageName) {
                analyzer = new Analyzer((ApplicationUnderTest)applicationUnderTest.Copy());
            }

            void Detail(string pageName, string commands) {
                var document = Document.Parse(commands);
                foreach (var instruction in document.Content.Branches) {
                    analyzer.Process((SlimTree)instruction);
                }
            }
        
            void EndPage(string pageName) {
                foreach (var call in analyzer.Calls) writer.WriteLine($"{pageName}|{call}");
            }

            IEnumerable<Tuple<string, string>> Lines() {
                while (true) {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    var splitPoint = line.IndexOf("|", StringComparison.Ordinal);
                    var pageName = line.Substring(0, splitPoint);
                    var commands = line.Substring(splitPoint + 1);
                    yield return new Tuple<string, string>(pageName, commands);
                }
            }

            readonly ApplicationUnderTest applicationUnderTest;
            readonly TextReader reader;
            readonly TextWriter writer;
            Analyzer analyzer;
        }
    }
    
}