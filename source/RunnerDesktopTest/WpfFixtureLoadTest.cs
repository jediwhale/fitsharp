// Copyright © 2026 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.RunnerDesktopTest {
    [TestFixture]
    public class WpfFixtureLoadTest {

        [Test]
        public void WindowsRunnerLoadsFixtureThatDependsOnWpf() {
            var tfm = new DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)).Name;
            var libraryTfm = tfm.Replace("-windows", string.Empty);
            var configuration = AppContext.BaseDirectory.Contains(Path.Combine("bin", "Debug"), StringComparison.OrdinalIgnoreCase)
                ? "Debug"
                : "Release";
            var root = FindRoot();
            var work = Path.Combine(Path.GetTempPath(), "fitsharp-wpf-" + Guid.NewGuid().ToString("N"));

            try {
                Directory.CreateDirectory(Path.Combine(work, "pages"));
                Directory.CreateDirectory(Path.Combine(work, "results"));
                Copy(Path.Combine(root, "source", "Runner", "bin", configuration, tfm), work, "Runner.dll");
                Copy(Path.Combine(root, "source", "Runner", "bin", configuration, tfm), work, "Runner.deps.json");
                Copy(Path.Combine(root, "source", "Runner", "bin", configuration, tfm), work, "Runner.runtimeconfig.json");
                Copy(Path.Combine(root, "source", "fit", "bin", configuration, libraryTfm), work, "fit.dll");
                Copy(Path.Combine(root, "source", "fitSharp", "bin", configuration, libraryTfm), work, "fitSharp.dll");
                Copy(Path.Combine(root, "source", "TestTargetWpf", "bin", configuration, tfm), work, "TestTargetWpf.dll");

                File.WriteAllText(Path.Combine(work, "pages", "Wpf.html"), Page);
                File.WriteAllText(Path.Combine(work, "suite.config.xml"), Config(work));

                var result = RunDotnet(work, "Runner.dll", "-c", "suite.config.xml");

                ClassicAssert.AreEqual(0, result.ExitCode,
                    "The windows runner could not run a fixture that depends on WPF." + Environment.NewLine + result.Output);
            }
            finally {
                try { Directory.Delete(work, true); }
                catch (IOException) { }
            }
        }

        const string Page = @"<html><body>
<table border=""1"" cellpadding=""2"" cellspacing=""0"">
<tr><td>fitSharp.TestTargetWpf.WpfFixture</td></tr>
<tr><td>wpfAssemblyName()</td></tr>
<tr><td>PresentationFramework</td></tr>
</table>
</body></html>";

        static string Config(string work) {
            var path = work.Replace('\\', '/');
            return $@"<suiteConfig>
  <Settings>
    <inputFolder>{path}/pages</inputFolder>
    <outputFolder>{path}/results</outputFolder>
    <runner>fit.Runner.FolderRunner</runner>
  </Settings>
  <ApplicationUnderTest>
    <addNamespace>fitSharp.TestTargetWpf</addNamespace>
    <addAssembly>{path}/TestTargetWpf.dll</addAssembly>
  </ApplicationUnderTest>
</suiteConfig>";
        }

        static void Copy(string fromDirectory, string toDirectory, string fileName) {
            var source = Path.Combine(fromDirectory, fileName);
            ClassicAssert.IsTrue(File.Exists(source), "Missing build output: " + source);
            File.Copy(source, Path.Combine(toDirectory, fileName), true);
        }

        static string FindRoot() {
            var directory = AppContext.BaseDirectory;
            var marker = Path.DirectorySeparatorChar + "source" + Path.DirectorySeparatorChar;
            var index = directory.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0) throw new InvalidOperationException("Cannot find the repository root from " + directory);
            return directory.Substring(0, index);
        }

        static (int ExitCode, string Output) RunDotnet(string work, params string[] arguments) {
            var start = new ProcessStartInfo(HostPath()) {
                WorkingDirectory = work,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            foreach (var argument in arguments) start.ArgumentList.Add(argument);
            using var process = Process.Start(start);
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            if (!process.WaitForExit(120000)) {
                process.Kill(true);
                process.WaitForExit();
                ClassicAssert.Fail("The runner did not finish within two minutes.");
            }
            return (process.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
        }

        static string HostPath() {
            return Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
        }
    }
}
