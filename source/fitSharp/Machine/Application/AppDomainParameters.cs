using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Machine.Model;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace fitSharp.Machine.Application {
    public class AppDomainParameters : IAppDomainSetup, Copyable {

        public AppDomainParameters() {
            this.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
        }

        #region IAppDomainSetup Members
        public string ApplicationBase { get; set; }
        public string ApplicationName { get; set; }
        public string CachePath { get; set; }
        public string ConfigurationFile { get; set; }
        public string DynamicBase { get; set; }
        public string LicenseFile { get; set; }
        public string PrivateBinPath { get; set; }
        public string PrivateBinPathProbe { get; set; }
        public string ShadowCopyDirectories { get; set; }
        public string ShadowCopyFiles { get; set; }
        #endregion

        public void CopyTo(IAppDomainSetup other) {
            other.ApplicationBase = this.ApplicationBase;
            other.ApplicationName = this.ApplicationName;
            other.CachePath = this.CachePath;
            other.ConfigurationFile = this.ConfigurationFile;
            other.DynamicBase = this.DynamicBase;
            other.LicenseFile = this.LicenseFile;
            other.PrivateBinPath = this.PrivateBinPath;
            other.PrivateBinPathProbe = this.PrivateBinPathProbe;
            other.ShadowCopyDirectories = this.ShadowCopyDirectories;
            other.ShadowCopyFiles = this.ShadowCopyFiles;
        }

        #region Copyable Members

        public Copyable Copy() {
            return (Copyable)this.MemberwiseClone();
        }

        #endregion

        public void Write(XmlWriter writer) {
            writer.WriteStartElement("AppDomainSetup");
            writer.WriteElementString("ApplicationBase", this.ApplicationBase);
            writer.WriteElementString("ApplicationName", this.ApplicationName);
            writer.WriteElementString("CachePath", this.CachePath);
            writer.WriteElementString("ConfigurationFile", this.ConfigurationFile);
            writer.WriteElementString("DynamicBase", this.DynamicBase);
            writer.WriteElementString("LicenseFile", this.LicenseFile);
            writer.WriteElementString("PrivateBinPath", this.PrivateBinPath);
            writer.WriteElementString("PrivateBinPathProbe", this.PrivateBinPathProbe);
            writer.WriteElementString("ShadowCopyDirectories", this.ShadowCopyDirectories);
            writer.WriteElementString("ShadowCopyFiles", this.ShadowCopyFiles);
            writer.WriteEndDocument();
        }

        public static AppDomainParameters Read(XmlReader reader) {
            AppDomainParameters rv = new AppDomainParameters();
            XmlDocument xd = new XmlDocument();
            xd.Load(reader);

            foreach (XmlNode node in xd.DocumentElement.ChildNodes) {
                XmlElement el = node as XmlElement;
                if (el != null) {
                    switch (el.LocalName) {
                        case "ApplicationBase":
                            rv.ApplicationBase = el.InnerText;
                            break;
                        case "ApplicationName":
                            rv.ApplicationName = el.InnerText;
                            break;
                        case "CachePath":
                            rv.CachePath = el.InnerText;
                            break;
                        case "ConfigurationFile":
                            rv.ConfigurationFile = el.InnerText;
                            break;
                        case "DynamicBase":
                            rv.DynamicBase = el.InnerText;
                            break;
                        case "LicenseFile":
                            rv.LicenseFile = el.InnerText;
                            break;
                        case "PrivateBinPath":
                            rv.PrivateBinPath = el.InnerText;
                            break;
                        case "PrivateBinPathProbe":
                            rv.PrivateBinPathProbe = el.InnerText;
                            break;
                        case "ShadowCopyDirectories":
                            rv.ShadowCopyDirectories = el.InnerText;
                            break;
                        case "ShadowCopyFiles":
                            rv.ShadowCopyFiles = el.InnerText;
                            break;
                    }
                }
            }

            return rv;
        }

        public static AppDomainParameters Read(XmlReader reader, string originalFilePath) {
            //it is convenient to have the runner interpret the ApplicationBase and ConfigurationFile paths
            //as being relative to the setup file.  this is useful for working with a project that has multiple
            //branches
            AppDomainParameters rv = Read(reader);
            if (!string.IsNullOrEmpty(originalFilePath)) {
                string baseDir = Path.GetDirectoryName(Path.GetFullPath(originalFilePath));

                if (!string.IsNullOrEmpty(rv.ApplicationBase) && !Path.IsPathRooted(rv.ApplicationBase))
                    rv.ApplicationBase = Path.GetFullPath(Path.Combine(baseDir, rv.ApplicationBase));

                if (!string.IsNullOrEmpty(rv.ConfigurationFile) && !Path.IsPathRooted(rv.ConfigurationFile))
                    rv.ConfigurationFile = Path.GetFullPath(Path.Combine(baseDir, rv.ConfigurationFile));
            }
            return rv;
        }

        public static AppDomainParameters Read(string parameterFile) {
            using (Stream fin = new FileStream(parameterFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (XmlReader reader = XmlReader.Create(fin)) {
                return Read(reader, parameterFile);
            }
        }
    }
}
