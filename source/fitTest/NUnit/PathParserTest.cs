// Copyright � 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fitnesse.fitserver;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class PathParserTest
    {
        [Test]
        public void TestEmptyString()
        {
            PathParser parser = new PathParser("");
            ClassicAssert.AreEqual(0, parser.AssemblyPaths.Count());
        }

        [Test]
        public void TestNull()
        {
            PathParser parser = new PathParser(null);
            verifyAssemblyPaths(new string[]{}, parser.AssemblyPaths);
        }

        [Test]
        public void TestOneAssembly()
        {
            string path = "d:\\path\\to\\assembly.dll";
            PathParser parser = new PathParser(path);
            verifyAssemblyPaths(new []{path}, parser.AssemblyPaths);
        }

        [Test]
        public void TestTwoAssemblies()
        {
            string path1 = "d:\\path\\to\\assembly.dll";
            string path2 = "d:\\path\\to\\another\\assembly.dll";
            string paths = path1 + ";" + path2;
            PathParser parser = new PathParser(paths);
            verifyAssemblyPaths(new []{path1, path2}, parser.AssemblyPaths);
        }

        [Test]
        public void TestOneConfigFile()
        {
            string path = "d:\\path\\to\\assembly.dll.config";
            PathParser parser = new PathParser(path);
            verifyAssemblyPaths(new string[]{}, parser.AssemblyPaths);
        }

        [Test]
        public void TestHandlesRelativePath()
        {
            string path = "fake.config";
            PathParser parser = new PathParser(path);
            verifyAssemblyPaths(new string[]{}, parser.AssemblyPaths);
        }

        [Test]
        public void TestExceptionOnSecondConfigFile()
        {
            string path1 = "d:\\path\\to\\assembly.dll.config";
            string path2 = "d:\\path\\to\\another\\assembly.dll.config";
            string paths = path1 + ";" + path2;
            var parser = new PathParser(paths);
            verifyAssemblyPaths(new string[]{}, parser.AssemblyPaths);
        }

        public void TestOneConfigFileAndOneAssemblyFile()
        {
            string assemblyPath = "d:\\path\\to\\assembly.dll";
            string configFilePath = "d:\\path\\to\\assembly.dll.config";
            string paths = assemblyPath + ";" + configFilePath;
            PathParser parser = new PathParser(paths);
            verifyAssemblyPaths(new []{assemblyPath}, parser.AssemblyPaths);
        }

        private void verifyAssemblyPaths(string[] expected, IEnumerable<string> list2)
        {
            IList list1 = new ArrayList(expected);
            ClassicAssert.AreEqual(list1.Count, list2.Count());
            foreach (string obj in list1)
                ClassicAssert.IsTrue(list2.Contains(obj));
        }


    }
}