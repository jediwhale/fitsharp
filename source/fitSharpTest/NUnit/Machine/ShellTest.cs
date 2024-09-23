// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

#if !NETCOREAPP
using System;
using System.Configuration;
#endif
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ShellTest {

#if !NETCOREAPP
        [Test] public void CustomAppConfigIsUsed() {
            var result = RunShell(new[] {"-a", "fitSharpTest.dll.alt.config",
                "-r", typeof (SampleRunner).FullName + "," + typeof (SampleRunner).Assembly.CodeBase} );
            ClassicAssert.AreEqual(606, result);
        }

        [Test] public void CustomAppConfigIsLoadedRelativeToExecutingAssembly() {
            using (new PushCurrentDirectory(@"\")) {
                CustomAppConfigIsUsed();
            }
        }

        [Test] public void CustomAppConfigFromSuiteConfigIsUsed() {
            var folders = new FolderTestModel();
            folders.MakeFile("suite.config.xml", "<config><Settings><AppConfigFile>fitSharpTest.dll.alt.config</AppConfigFile></Settings></config>");
            folders.MakeFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fitSharpTest.dll.alt.config"), "stuff");
            var result = RunShell(new[] {"-c", "suite.config.xml",
                "-r", typeof (SampleRunner).FullName + "," + typeof (SampleRunner).Assembly.CodeBase}, folders );
            ClassicAssert.AreEqual(606, result);
        }

        class PushCurrentDirectory : IDisposable {
            public PushCurrentDirectory(string directory) {
                original = Environment.CurrentDirectory;
                Environment.CurrentDirectory = directory;
            }

            public void Dispose() {
                Environment.CurrentDirectory = original;
            }

            readonly string original;
        }
#endif

        [Test] public void RunnerIsCalled() {
            var result = RunShell(new [] {"-r", typeof(SampleRunner).FullName});
            ClassicAssert.AreEqual(SampleRunner.Result, result);
        }

        [Test] public void AdditionalArgumentsArePassed() {
            RunShell(new [] {"more", "-r", typeof(SampleRunner).FullName, "stuff"});
            ClassicAssert.AreEqual(2, SampleRunner.LastArguments.Count);
            ClassicAssert.AreEqual("more", SampleRunner.LastArguments[0]);
            ClassicAssert.AreEqual("stuff", SampleRunner.LastArguments[1]);
        }

        [Test] public void RunnerFromSuiteConfigIsUsed() {
            var folders = new FolderTestModel();
            folders.MakeFile("suite.config.xml", "<config><Settings><Runner>"
                + typeof (SampleRunner).FullName + ","
                + TargetFramework.Location(typeof (SampleRunner).Assembly)
                + "</Runner></Settings></config>");
            var result = RunShell(new[] {"-c", "suite.config.xml"}, folders );
            ClassicAssert.AreEqual(SampleRunner.Result, result);
        }

        [Test] public void ApartmentStateFromSuiteConfigIsUsed() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return; //apartment state only supported on windows
            var folders = new FolderTestModel();
            folders.MakeFile("suite.config.xml", "<config><Settings><ApartmentState>STA</ApartmentState></Settings></config>");
            RunShell(new[] {"-r", typeof(SampleRunner).FullName, "-c", "suite.config.xml"}, folders );
            ClassicAssert.AreEqual(ApartmentState.STA, SampleRunner.ApartmentState);
        }

        static int RunShell(IList<string> arguments) {
            return RunShell(arguments, new FileSystemModel());
        }

        static int RunShell(IList<string> arguments, FolderModel model) {
            return new Shell(new NullReporter(), new ShellArguments(model, arguments)).Run();
        }
    }

    public class SampleRunner: Runnable {
        public const int Result = 707;

        public static IList<string> LastArguments;
        public static ApartmentState ApartmentState;

        public SampleRunner() {
            LastArguments = new string[] {};
        }

        public int Run(IList<string> arguments, Memory memory, ProgressReporter reporter) {
            LastArguments = arguments;
            ApartmentState = Thread.CurrentThread.GetApartmentState();
#if NETCOREAPP
            return Result;
#else
            try {
                return int.Parse(ConfigurationManager.AppSettings.Get("returnCode"));
            }
            catch (Exception) {
                return Result;
            }
#endif
        }
    }
}
