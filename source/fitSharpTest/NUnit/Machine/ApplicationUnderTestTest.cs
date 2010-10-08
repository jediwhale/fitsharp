// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ApplicationUnderTestTest {
        private ApplicationUnderTest applicationUnderTest;

        [SetUp] public void SetUp() {
            applicationUnderTest = new ApplicationUnderTest();
        }

        [Test] public void TypeIsFoundInCurrentAssembly() {
            CheckTypeFound<SampleClass>(typeof(SampleClass).FullName);
        }

        [Test] public void TypeWithoutNamespaceIsFound() {
            CheckTypeFound<AnotherSampleClass>("AnotherSampleClass");
        }

        [Test] public void TypeIsFoundUsingNamespaces() {
            CheckTypeNotFound("SampleClass");
            applicationUnderTest.AddNamespace(typeof(SampleClass).Namespace);
            CheckTypeFound<SampleClass>("SampleClass");
        }

        [Test] public void FindsTypeUsingCaseInsenstiveNamespace() {
            CheckTypeNotFound("SampleClass");
            applicationUnderTest.AddNamespace(typeof(SampleClass).Namespace.ToLower());
            CheckTypeFound<SampleClass>("SampleClass");
        }

        [Test] public void NamespaceIsTrimmed() {
            applicationUnderTest.AddNamespace(" " + typeof(SampleClass).Namespace + "\n");
            CheckTypeFound<SampleClass>("SampleClass");
        }

        [Test] public void TypeIsFoundInLoadedAssembly() {
            applicationUnderTest.AddAssembly("testtarget.dll");
            RuntimeType sample = GetType("fitSharp.TestTarget.SampleDomain");
            Assert.AreEqual("fitSharp.TestTarget.SampleDomain", sample.Type.FullName);
        }

        [Test] public void ReloadingAssemblyIsIgnored() {
            applicationUnderTest.AddAssembly("testtarget.dll");
            applicationUnderTest.AddAssembly("testtarget.dll");
            RuntimeType sample = GetType("fitSharp.TestTarget.SampleDomain");
            Assert.AreEqual("fitSharp.TestTarget.SampleDomain", sample.Type.FullName);
        }

        [Test] public void JarFilesAreIgnored() {
            applicationUnderTest.AddAssembly("testtarget.jAr");
            Assert.IsTrue(true);
        }

        [Test] public void TypeIsFoundInDefaultNamespace() {
            CheckTypeFound<ApplicationUnderTest>("ApplicationUnderTest");
        }

        [Test] public void NamespaceIsRemoved() {
            applicationUnderTest.AddNamespace(typeof(SampleClass).Namespace);
            CheckTypeFound<SampleClass>("SampleClass");
            applicationUnderTest.RemoveNamespace(typeof(SampleClass).Namespace);
            CheckTypeNotFound("SampleClass");
        }

        [Test] public void ChangesNotMadeInCopy() {
            var copy = (ApplicationUnderTest)applicationUnderTest.Copy();
            applicationUnderTest.AddNamespace("fitnesse.unitTest.engine");
            applicationUnderTest = copy;
            CheckTypeNotFound("SampleClass");
        }

        [Test]
        public void AddAssemblyNameToExceptionContext() {
            ArgumentException ex = Error.Expect<ArgumentException>(() => applicationUnderTest.AddAssembly("<span"));
            Assert.That(ex.Data.Values, Has.Member("<span"));
        }

        void CheckTypeFound<T>(string typeName) {
            RuntimeType sample = GetType(typeName);
            Assert.AreEqual(typeof(T), sample.Type);
        }

        void CheckTypeNotFound(string typeName) {
            string message = string.Empty;
            try {
                GetType(typeName);
            }
            catch (Exception e) {
                message = e.Message;
            }
            Assert.IsTrue(message.StartsWith("Type 'SampleClass' not found in assemblies"));
        }

        RuntimeType GetType(string name) {
            return applicationUnderTest.FindType(new IdentifierName(name));
        }
    }
}

public class AnotherSampleClass {}
