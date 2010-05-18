using System;
using System.IO;
using System.Xml;
using fitSharp.Machine.Application;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace fitSharp.Test.NUnit.Application {
    [TestFixture]
    public class AppDomainParametersTest {
        [Test]
        public void CopyTo_copies_all_parameters_to_new_AppDomainSetup() {
            AppDomainParameters parms = CreateParameters();
            AppDomainParameters newInstance = new AppDomainParameters();

            parms.CopyTo(newInstance);
            AssertAllPropertiesAreEqual(parms, newInstance);

            AppDomainSetup setup = new AppDomainSetup();
            parms.CopyTo(setup);

            //AppDomainSetup does its own thing with the values that are copied and we don't control
            //that (or even want to try to control it; not our responsiblity), which is why the 
            //result is not verified. because they use the same interface, the first assert ensures
            //that the values get copied.
        }

        [Test]
        public void WriteTo_then_Read_is_identity_transform() {
            AppDomainParameters parms = CreateParameters();
            AppDomainParameters newInstance = null;

            StringWriter textWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(textWriter)) {
                parms.Write(writer);
            }
            using (XmlReader reader = XmlReader.Create(new StringReader(textWriter.ToString()))) {
                newInstance = AppDomainParameters.Read(reader);
            }

            AssertAllPropertiesAreEqual(parms, newInstance);
        }

        [Test]
        public void Copy_returns_object_with_same_parameter_values() {
            AppDomainParameters parms = CreateParameters();
            AppDomainParameters newInstance = (AppDomainParameters)parms.Copy();

            AssertAllPropertiesAreEqual(parms, newInstance);
        }

        [Test]
        public void Read_from_file_converts_relative_paths_to_absolute_paths() {
            AppDomainParameters parms = null;
            StringWriter textWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(textWriter)) {
                new AppDomainParameters {
                    ApplicationBase = @"..",
                    ApplicationName = "foo",
                    CachePath = @"C:\cache\path",
                    ConfigurationFile = @".\fitnesse.config",
                    DynamicBase = @"C:\dir\to\store\dynamic-proxies",
                    LicenseFile = "yuck",
                    PrivateBinPath = @".\bin",
                    PrivateBinPathProbe = @".\lib;tools\fitsharp",
                    ShadowCopyDirectories = @"fitnesse\shadow-copies",
                    ShadowCopyFiles = "true"
                }.Write(writer);
            }

            string xml = textWriter.ToString();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml))) {
                parms = AppDomainParameters.Read(reader, @"C:\projects\project\main\fitnesse\app-domain.xml");
            }

            Assert.That(parms.ApplicationBase, Is.EqualTo(@"C:\projects\project\main"));
            Assert.That(parms.ConfigurationFile, Is.EqualTo(@"C:\projects\project\main\fitnesse\fitnesse.config"));
            Assert.That(parms.CachePath, Is.EqualTo(@"C:\cache\path"));
            Assert.That(parms.DynamicBase, Is.EqualTo(@"C:\dir\to\store\dynamic-proxies"));
            Assert.That(parms.PrivateBinPath, Is.EqualTo(@".\bin"));
            Assert.That(parms.PrivateBinPathProbe, Is.EqualTo(@".\lib;tools\fitsharp"));
            Assert.That(parms.ShadowCopyDirectories, Is.EqualTo(@"fitnesse\shadow-copies"));
        }

        [Test]
        public void default_value_of_ApplicationBase_is_current_domain_base() {
            Assert.That(new AppDomainParameters().ApplicationBase, Is.EqualTo(AppDomain.CurrentDomain.BaseDirectory));
        }

        //[Test]
        //public void foo() {
        //    using (XmlWriter writer = XmlWriter.Create(@"C:\oss\fitsharp\source\fitSharpTest\domainSetup.xml")) {
        //        CreateParameters().Write(writer);
        //        writer.Flush();
        //        writer.Close();
        //    }
        //}

        private void AssertAllPropertiesAreEqual(AppDomainParameters expected, AppDomainParameters actual) {
            Assert.That(actual.ApplicationBase, Is.EqualTo(expected.ApplicationBase));
            Assert.That(actual.ApplicationName, Is.EqualTo(expected.ApplicationName));
            Assert.That(actual.CachePath, Is.EqualTo(expected.CachePath));
            Assert.That(actual.ConfigurationFile, Is.EqualTo(expected.ConfigurationFile));
            Assert.That(actual.DynamicBase, Is.EqualTo(expected.DynamicBase));
            Assert.That(actual.LicenseFile, Is.EqualTo(expected.LicenseFile));
            Assert.That(actual.PrivateBinPath, Is.EqualTo(expected.PrivateBinPath));
            Assert.That(actual.PrivateBinPathProbe, Is.EqualTo(expected.PrivateBinPathProbe));
            Assert.That(actual.ShadowCopyDirectories, Is.EqualTo(expected.ShadowCopyDirectories));
            Assert.That(actual.ShadowCopyFiles, Is.EqualTo(expected.ShadowCopyFiles));
        }

        private AppDomainParameters CreateParameters() {
            return new AppDomainParameters {
                ApplicationBase = @"C:\my\project\main",
                ApplicationName = "foo",
                CachePath = @"C:\cache\path",
                ConfigurationFile = @"C:\my\project\main\fitnesse\fitnesse.config",
                DynamicBase = @"C:\dir\to\store\dynamic-proxies",
                LicenseFile = "yuck",
                PrivateBinPath = @"C:\my\project\main\bin",
                PrivateBinPathProbe = @"C:\my\project\main\lib;C:\my\project\main\tools\fitsharp",
                ShadowCopyDirectories = @"C:\temp\project-shadow",
                ShadowCopyFiles = "true"
            };
        }
    }
}
