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
            //result of copying to System.AppDomainSetup is not verified. The first assertion ensures
            //that the correct values are set.
        }

        [Test]
        public void Write_then_Read_is_identity_transform() {
            AppDomainParameters parms = CreateParameters();
            AppDomainParameters newInstance = null;

            StringWriter textWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true })) {
                parms.Write(writer);
            }
            string xml = textWriter.ToString();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml))) {
                newInstance = AppDomainParameters.Read(reader);
            }

            AssertAllPropertiesAreEqual(parms, newInstance);
        }

        [Test]
        public void Read_throws_when_unrecognized_element_name() {
            AppDomainParameters parms = CreateParameters();

            StringWriter textWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true })) {
                parms.Write(writer);
            }
            string xml = textWriter.ToString();
            XmlDocument xd = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml))) {
                xd.Load(reader);
            }

            XmlElement el = xd.CreateElement("gobbldey-gook");
            el.InnerText = "asdfas";
            xd.DocumentElement.AppendChild(el);
            textWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true })) {
                xd.WriteContentTo(writer);
            }
            xml = textWriter.ToString();

            using (XmlReader reader = XmlReader.Create(new StringReader(xml))) {
                Error.Expect<InvalidOperationException>(() => AppDomainParameters.Read(reader));
            }
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
                    CachePath = @"cache",
                    ConfigurationFile = @".\fitnesse.config",
                    DynamicBase = @"dynamic-proxies",
                    LicenseFile = "yuck",
                    PrivateBinPath = @"..\lib;tools\fitsharp",
                    ShouldExcludeApplicationBaseFromAssemblyProbe = true,
                    ShadowCopyDirectories = @".\shadow-copies",
                    ShadowCopyFiles = "true"
                }.Write(writer);
            }

            string xml = textWriter.ToString();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml))) {
                parms = AppDomainParameters.Read(reader, @"C:\projects\project\trunk\fitnesse\app-domain.xml");
            }

            Assert.That(parms.ApplicationBase, Is.EqualTo(@"C:\projects\project\trunk"));
            Assert.That(parms.ConfigurationFile, Is.EqualTo(@"C:\projects\project\trunk\fitnesse\fitnesse.config"));
            Assert.That(parms.CachePath, Is.EqualTo(@"C:\projects\project\trunk\fitnesse\cache"));
            Assert.That(parms.DynamicBase, Is.EqualTo(@"C:\projects\project\trunk\fitnesse\dynamic-proxies"));
            Assert.That(parms.PrivateBinPath, Is.EqualTo(@"C:\projects\project\trunk\lib;C:\projects\project\trunk\fitnesse\tools\fitsharp"));
            Assert.That(parms.ShouldExcludeApplicationBaseFromAssemblyProbe, Is.True);
            Assert.That(parms.ShadowCopyDirectories, Is.EqualTo(@"C:\projects\project\trunk\fitnesse\shadow-copies"));
        }

        [Test]
        public void setter_throws_when_PrivateBinPath_contains_dirs_not_under_ApplicationBase() {
            AppDomainParameters parms = new AppDomainParameters();
            parms.ApplicationBase = @"C:\foo";
            Error.Expect<InvalidOperationException>(() => parms.PrivateBinPath = @"bin;C:\bar\bin");
        }

        [Test]
        public void Read_throws_when_PrivateBinPath_contains_dirs_not_under_ApplicationBase() {
            using (StringReader sr = new StringReader("<app><ApplicationBase>C:\\foo</ApplicationBase><PrivateBinPath>bin;C:\\bar\\bin</PrivateBinPath></app>"))
            using (XmlReader reader = XmlReader.Create(sr)) {
                Error.Expect<InvalidOperationException>(() => AppDomainParameters.Read(reader));
            }
            using (StringReader sr = new StringReader("<app><PrivateBinPath>bin;C:\\bar\\bin</PrivateBinPath><ApplicationBase>C:\\foo</ApplicationBase></app>"))
            using (XmlReader reader = XmlReader.Create(sr)) {
                Error.Expect<InvalidOperationException>(() => AppDomainParameters.Read(reader));
            }
        }

        [Test]
        public void PrivateBinPathProbe_is_empty_string_when_ShouldExcludeApplicationBaseFromAssemblyProbe_is_false() {
            AppDomainParameters parms = CreateParameters();
            parms.ShouldExcludeApplicationBaseFromAssemblyProbe = false;

            string actual = ((IAppDomainSetup)parms).PrivateBinPathProbe;
            Assert.That(actual, Is.EqualTo(""));
        }

        [Test]
        public void PrivateBinPathProbe_is_non_empty_string_when_ShouldExcludeApplicationBaseFromAssemblyProbe_is_true() {
            AppDomainParameters parms = CreateParameters();
            parms.ShouldExcludeApplicationBaseFromAssemblyProbe = true;

            string actual = ((IAppDomainSetup)parms).PrivateBinPathProbe;
            Assert.That(actual.Length, Is.GreaterThan(0));
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
            Assert.That(actual.ShouldExcludeApplicationBaseFromAssemblyProbe, Is.EqualTo(expected.ShouldExcludeApplicationBaseFromAssemblyProbe));
            Assert.That(actual.ShadowCopyDirectories, Is.EqualTo(expected.ShadowCopyDirectories));
            Assert.That(actual.ShadowCopyFiles, Is.EqualTo(expected.ShadowCopyFiles));
        }

        private AppDomainParameters CreateParameters() {
            return new AppDomainParameters {
                ApplicationBase = @"C:\my\project\trunk",
                ApplicationName = "foo",
                CachePath = @"C:\cache\path",
                ConfigurationFile = @"C:\my\project\trunk\fitnesse\fitnesse.config",
                DynamicBase = @"C:\dir\to\store\dynamic-proxies",
                LicenseFile = "yuck",
                PrivateBinPath = @"C:\my\project\trunk\bin",
                ShouldExcludeApplicationBaseFromAssemblyProbe = true,
                ShadowCopyDirectories = @"C:\temp\project-shadow",
                ShadowCopyFiles = "true"
            };
        }
    }
}
