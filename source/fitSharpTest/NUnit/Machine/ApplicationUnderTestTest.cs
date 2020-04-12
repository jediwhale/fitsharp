// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Reflection;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using Moq;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ApplicationUnderTestTest {
        ApplicationUnderTest applicationUnderTest;
        Mock<ApplicationDomain> domain;

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            CopySample("TestTarget");
            CopySample("TestTarget2");
        }

        void CopySample(string sampleName) {
            var target = Path.Combine(TargetPath(), "build", "sample", sampleName);
            Directory.CreateDirectory(target);
            var source = Assembly.GetExecutingAssembly().Location.Replace("fitSharpTest", sampleName);
            File.Copy(source, Path.Combine(target, sampleName + ".dll"), true);
        }

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

        [Test] public void NamespaceIsRemoved() {
            applicationUnderTest.AddNamespace(typeof(SampleClass).Namespace);
            CheckTypeFound<SampleClass>("SampleClass");
            applicationUnderTest.RemoveNamespace(typeof(SampleClass).Namespace);
            CheckTypeNotFound("SampleClass");
        }

        [Test] public void TypeIsFoundInLoadedAssembly() {
            LoadTestTarget();
            RuntimeType sample = GetType("fitSharp.TestTarget.SampleDomain");
            Assert.AreEqual("fitSharp.TestTarget.SampleDomain", sample.Type.FullName);
        }

        [Test] public void TypeIsFoundInDefaultNamespace() {
            CheckTypeFound<ApplicationUnderTest>("ApplicationUnderTest");
        }

        [Test] public void ChangesNotMadeInCopy() {
            var copy = (ApplicationUnderTest)applicationUnderTest.Copy();
            applicationUnderTest.AddNamespace(typeof(SampleClass).Namespace);
            CheckTypeFound<SampleClass>("SampleClass");
            applicationUnderTest = copy;
            CheckTypeNotFound("SampleClass");
        }

        [Test] public void ReloadingAssemblyIsIgnored() {
            LoadTestTarget();
            LoadTestTarget();
            RuntimeType sample = GetType("fitSharp.TestTarget.SampleDomain");
            Assert.AreEqual("fitSharp.TestTarget.SampleDomain", sample.Type.FullName);
        }

        [Test] public void JarFilesAreIgnored() {
            SetUpMockDomain();
            applicationUnderTest.AddAssembly("testtarget.jAr");
            domain.Verify(d => d.LoadAssembly(It.IsAny<string>()), Times.Never());
            Assert.IsTrue(true);
        }

        [Test]
        public void LoadsFromAlternateFolder() {
            AssemblyLoadFailureHandler.AddFolder(Path.Combine(TargetPath(), "build/sample/TestTarget".AsPath()));
            LoadTarget("TestTarget2");
            var runtimeType = applicationUnderTest.FindType("fitSharp.TestTarget2.SampleWithDependency");
            Assert.AreEqual("my sample says hi sample", runtimeType.CreateInstance().ValueString);
        }

        void SetUpMockDomain() {
            domain = new Mock<ApplicationDomain>();
            applicationUnderTest = new ApplicationUnderTest(domain.Object);
        }

        [Test] public void FitAssemblyIsLoadedForFitClass() {
            ExpectFitAssemblyIsLoaded();
            applicationUnderTest.FindType(new IdentifierName("fit.FitSample"));
            domain.VerifyAll();
        }

        [Test] public void OptionalAssemblyIsLoaded() {
            SetUpMockDomain();
            applicationUnderTest.AddOptionalAssembly("myassembly.dll");
            domain.Verify(d => d.LoadAssembly("myassembly.dll"), Times.Once());
        }

        [Test] public void OptionalAssemblyIgnoresMissingAssembly() {
            SetUpMockDomain();
            domain.Setup(d => d.LoadAssembly("myassembly.dll")).Throws(new FileNotFoundException());
            applicationUnderTest.AddOptionalAssembly("myassembly.dll");
            Assert.IsTrue(true);
        }

        void ExpectFitAssemblyIsLoaded() {
            SetUpMockDomain();
            var types = new Mock<Types>();
            types.Setup(t => t.Types).Returns(new [] {typeof (fit.FitSample)});
            domain
                .Setup(a => a.LoadAssembly(It.Is<string>(s => s.EndsWith("/fit.dll", StringComparison.OrdinalIgnoreCase))))
                .Returns(types.Object);
        }

        [Test] public void FitAssemblyIsLoadedForFitNamespace() {
            ExpectFitAssemblyIsLoaded();
            applicationUnderTest.AddNamespace("fit");
            domain.VerifyAll();
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

        void LoadTestTarget() {
            LoadTarget("TestTarget");
        }

        void LoadTarget(string name) {
            applicationUnderTest.AddAssembly(Path.Combine(
                TargetPath(),
                ("build/sample/" + name + "/" + name + ".dll").AsPath()));
        }

        string TargetPath() {
            return AppDomain.CurrentDomain.BaseDirectory.Before(new[] {"/build/".AsPath(), "/source/".AsPath()});
        }
    }
}

public class AnotherSampleClass {}

namespace fit { public class FitSample {} }

