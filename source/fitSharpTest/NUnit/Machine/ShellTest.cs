// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Configuration;
using fitSharp.Machine.Application;
using NUnit.Framework;
using Configuration=fitSharp.Machine.Application.Configuration;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ShellTest {
        [Test] public void RunnerIsCalled() {
            int result = new Shell().Run(new [] {"-r", typeof(SampleRunner).FullName});
            Assert.AreEqual(SampleRunner.Result, result);
        }

        [Test] public void AdditionalArgumentsArePassed() {
            new Shell().Run(new [] {"more", "-r", typeof(SampleRunner).FullName, "stuff"});
            Assert.AreEqual(2, SampleRunner.LastArguments.Length);
            Assert.AreEqual("more", SampleRunner.LastArguments[0]);
            Assert.AreEqual("stuff", SampleRunner.LastArguments[1]);
        }

        [Test] public void CustomAppConfigIsUsed() {
            int result = new Shell().Run(new[] {"-a", "fitSharpTest.dll.alt.config",
                "-r", typeof (SampleRunner).FullName + "," + typeof (SampleRunner).Assembly.CodeBase} );
            Assert.AreEqual(606, result);
        }
    }

    public class SampleRunner: Runnable {
        public const int Result = 707;

        public static string[] LastArguments;

        public SampleRunner() {
            LastArguments = new string[] {};
        }

        public int Run(string[] arguments, Configuration configuration, ProgressReporter reporter) {
            LastArguments = arguments;
            try {
                return int.Parse(ConfigurationManager.AppSettings.Get("returnCode"));
            }
            catch (Exception) {
                return Result;
            }
        }
    }
}