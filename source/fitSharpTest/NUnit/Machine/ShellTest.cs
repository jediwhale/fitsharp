// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Configuration;
using fitSharp.IO;
using fitSharp.Machine.Application;
using NUnit.Framework;
using Configuration=fitSharp.Machine.Application.Configuration;
using System.IO;
using System.Xml;
using NUnit.Framework.SyntaxHelpers;

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

        [Test] public void CustomAppDomainSetupIsUsed() {
            string baseDir = typeof(VerifyAppDomainRunner).Assembly.CodeBase.Substring(8);
            baseDir = Path.GetDirectoryName(Path.GetFullPath(baseDir));
            AppDomainParameters parms = new AppDomainParameters();
            parms.ApplicationBase = "..";
            parms.ApplicationName = "CustomAppDomainSetupIsUsed-test-domain";
            parms.CachePath = Path.Combine(baseDir, "cache-tmp");
            parms.ConfigurationFile = Path.Combine(baseDir, "fitSharpTest.dll.alt.config");
            parms.DynamicBase = Path.Combine(baseDir, "dynamic-tmp");
            parms.PrivateBinPath = Path.GetFileName(baseDir);
            parms.PrivateBinPathProbe = "-foo";
            using (XmlWriter writer = XmlWriter.Create("domainSetup.xml")) {
                parms.Write(writer);
                writer.Flush();
                writer.Close();
            }
            
            int result = new Shell().Run(new[] {"-d", "domainSetup.xml",
                "-r", typeof (VerifyAppDomainRunner).FullName + "," + typeof (VerifyAppDomainRunner).Assembly.CodeBase});
            Assert.That(result, Is.EqualTo(0));
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

    public class VerifyAppDomainRunner : Runnable {

        #region Runnable Members

        public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
            IAppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            string baseDir = typeof(VerifyAppDomainRunner).Assembly.CodeBase.Substring(8);
            baseDir = Path.GetDirectoryName(Path.GetFullPath(baseDir));

            if (setup.ApplicationBase != baseDir)
                return 1;
            else if (setup.ApplicationName != "CustomAppDomainSetupIsUsed-test-domain")
                return 2;
            else if (setup.CachePath != Path.Combine(baseDir, "cache-tmp"))
                return 3;
            else if (setup.ConfigurationFile != Path.Combine(baseDir, "fitSharpTest.dll.alt.config"))
                return 4;
            else if (!setup.DynamicBase.StartsWith(Path.Combine(baseDir, "dynamic-tmp")))
                return 5;
            else if (setup.PrivateBinPath != Path.GetFileName(baseDir))
                return 6;
            else if (setup.PrivateBinPathProbe != "foo")
                return 7;
            else
                return 0;
        }

        #endregion
    }
}