// Copyright © 2025 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.StoryTest {
    [TestFixture]
    public class StoryTest {

        [Test]
        public void Run() {
            var root = RepositoryRoot();
            var build = new Uri(TargetFramework.Location(Assembly.GetExecutingAssembly())).LocalPath;
            var sourcePath = Path.GetDirectoryName(build);
            var destinationPath = Path.Combine(root, "build", "sandbox");
            Directory.CreateDirectory(destinationPath);
            Copy("fit", sourcePath, destinationPath);
            Copy("fitSharp", sourcePath, destinationPath);
            Copy("fitTest", sourcePath, destinationPath);
            Copy("fitSharpTest", sourcePath, destinationPath);
            Copy("Runner", sourcePath, destinationPath);
            Copy("Samples", sourcePath, destinationPath);
            Environment.CurrentDirectory = root;
            var config = Path.Combine(root, "storytest.config." + TargetFramework.FileExtension + ".xml");
            var output = Path.Combine(root, Setting(config, "outputFolder"));
            if (Directory.Exists(output)) Directory.Delete(output, true);

            ClassicAssert.AreEqual(0,  Shell.Run(new [] {"-c", config}));

            var page = Path.Combine(output, "Fit.html");
            ClassicAssert.IsTrue(
                File.Exists(page),
                "The story test did not run the suite: " + page + " is missing.");
        }

        static string Setting(string config, string name) {
            return XDocument.Load(config).Root.Element("Settings").Element(name).Value;
        }

        static string RepositoryRoot() {
            var directory = AppContext.BaseDirectory;
            var marker = Path.DirectorySeparatorChar + "source" + Path.DirectorySeparatorChar;
            var index = directory.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0) throw new InvalidOperationException("Cannot find the repository root from " + directory);
            return directory.Substring(0, index);
        }

        static void Copy(string project, string sourcePath, string destinationPath) {
            foreach (var file in Directory.GetFiles(sourcePath.Replace("StoryTest", project))) {
                if (Path.GetFileName(file).StartsWith(project)) {
                    File.Copy(file, Path.Combine(destinationPath, Path.GetFileName(file)), true);
                }
            }
        }
    }
}
