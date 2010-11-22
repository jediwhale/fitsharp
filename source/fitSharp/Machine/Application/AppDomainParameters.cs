using System;
using System.IO;
using System.Xml;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Application {
    /// <summary>
    /// Specifies how you want the app domain where your test code runs to be created.  When this class is
    /// read from an xml file using any of the Read overloads that accept a file name, all relative paths in
    /// the resulting object are interpreted as being relative to the directory of the xml file.
    /// </summary>
    public class AppDomainParameters : IAppDomainSetup, Copyable {

        private string _privateBinPath;

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
        /// <summary>
        /// Should .dll's contained in the ApplicationBase directory be excluded when resolving references?
        /// </summary>
        public bool ShouldExcludeApplicationBaseFromAssemblyProbe { get; set; }
        string IAppDomainSetup.PrivateBinPathProbe {
            //this is is a wierd animal.  it could probably have a better name and an appropriate data type, which
            //is the reason for explicit interface impl.
            //
            //http://msdn.microsoft.com/en-us/library/system.appdomainsetup.privatebinpathprobe%28v=VS.90%29.aspx
            //AppDomainSetup.PrivateBinPathProbe Property
            //  A String that is blank if ApplicationBase is included in the application search path, or nonblank
            //  if it is excluded. Set this property to any nonblank string to exclude the application directory
            //  path — that is, ApplicationBase  — from the search path for the application, and search only
            //  PrivateBinPath.
            //
            //this class is constructed such that ApplicationBase is searched unless you explicitly configure it
            //to be excluded
            get {
                if (this.ShouldExcludeApplicationBaseFromAssemblyProbe)
                    return "true";
                else
                    return "";
            }
            set {
                this.ShouldExcludeApplicationBaseFromAssemblyProbe = (value ?? "").Length > 0;
            }
        }
        public string PrivateBinPath {
            get { return _privateBinPath; }
            set {
                _privateBinPath = value;
                ValidatePrivateBinPaths();
            }
        }
        public string ShadowCopyDirectories { get; set; }
        public string ShadowCopyFiles { get; set; }
        #endregion

        private void ValidatePrivateBinPaths() {
            //the documentation on PrivateBinPath says that the paths will be ignored if not located under the
            //ApplicationBase directory.  it will be easier for people to figure out what is wrong if they get
            //a descriptive error instead of a TypeNotFound exception later on
            if (!string.IsNullOrEmpty(ApplicationBase) && !string.IsNullOrEmpty(_privateBinPath)) {
                string baseDir = ApplicationBase.ToLowerInvariant();
                string[] paths = _privateBinPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < paths.Length; i++) {
                    string p = paths[i].ToLowerInvariant();
                    if (Path.IsPathRooted(p) && !p.StartsWith(baseDir)) {
                        string msg = string.Format(
                            "PrivateBinPath element {0} is not valid.  It must be a child of {1}.",
                            i.ToString(),
                            this.ApplicationBase);
                        throw new InvalidOperationException(msg);
                    }
                }
            }
        }

        public void CopyTo(IAppDomainSetup other) {
            other.ApplicationBase = this.ApplicationBase;
            other.ApplicationName = this.ApplicationName;
            other.CachePath = this.CachePath;
            other.ConfigurationFile = this.ConfigurationFile;
            other.DynamicBase = this.DynamicBase;
            other.LicenseFile = this.LicenseFile;
            other.PrivateBinPath = this.PrivateBinPath;
            other.PrivateBinPathProbe = ((IAppDomainSetup)this).PrivateBinPathProbe;
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
            writer.WriteElementString("ShouldExcludeApplicationBaseFromAssemblyProbe", this.ShouldExcludeApplicationBaseFromAssemblyProbe.ToString());
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
                            rv.ApplicationBase = SafeTrim(el.InnerText);
                            break;
                        case "ApplicationName":
                            rv.ApplicationName = SafeTrim(el.InnerText);
                            break;
                        case "CachePath":
                            rv.CachePath = SafeTrim(el.InnerText);
                            break;
                        case "ConfigurationFile":
                            rv.ConfigurationFile = SafeTrim(el.InnerText);
                            break;
                        case "DynamicBase":
                            rv.DynamicBase = SafeTrim(el.InnerText);
                            break;
                        case "LicenseFile":
                            rv.LicenseFile = SafeTrim(el.InnerText);
                            break;
                        case "PrivateBinPath":
                            rv.PrivateBinPath = SafeTrim(el.InnerText);
                            break;
                        case "PrivateBinPathProbe":
                            ((IAppDomainSetup)rv).PrivateBinPathProbe = SafeTrim(el.InnerText);
                            break;
                        case "ShadowCopyDirectories":
                            rv.ShadowCopyDirectories = SafeTrim(el.InnerText);
                            break;
                        case "ShadowCopyFiles":
                            rv.ShadowCopyFiles = SafeTrim(el.InnerText);
                            break;
                        case "ShouldExcludeApplicationBaseFromAssemblyProbe":
                            rv.ShouldExcludeApplicationBaseFromAssemblyProbe = bool.Parse(SafeTrim(el.InnerText));
                            break;
                        default:
                            throw new InvalidOperationException("Unrecognized element in app domain setup: " + el.Name);
                    }
                }
            }

            rv.ValidatePrivateBinPaths();
            return rv;
        }

        private static string SafeTrim(string p) {
            if (p == null)
                return null;
            return p.Trim();
        }

        /// <summary>
        /// De-serializes an object stored as xml.  Uses the containing directory of the given
        /// file path as the base for all path calculations.
        /// </summary>
        public static AppDomainParameters Read(XmlReader reader, string originalFilePath) {
            //it is convenient to have the runner interpret the ApplicationBase, ConfigurationFile, and PrivateBinPath 
            //paths as being relative to the setup file.  this is useful for working with a project where the local
            //working copy has many branches nested under the project folder (similar to SVN) and where external dependencies
            //are under source control in binary format

            AppDomainParameters rv = Read(reader);
            if (string.IsNullOrEmpty(originalFilePath) && !string.IsNullOrEmpty(rv.ApplicationBase))
                originalFilePath = rv.ApplicationBase;

            if (!string.IsNullOrEmpty(originalFilePath)) {
                string baseDir = Path.GetDirectoryName(Path.GetFullPath(originalFilePath));

                if (string.IsNullOrEmpty(rv.ApplicationBase)) {
                    rv.ApplicationBase = baseDir;
                } else {
                    rv.ApplicationBase = NormalizePath(baseDir, rv.ApplicationBase);
                }

                rv.ConfigurationFile = NormalizePath(baseDir, rv.ConfigurationFile);
                rv.PrivateBinPath = NormalizePath(baseDir, rv.PrivateBinPath);
                rv.ShadowCopyDirectories = NormalizePath(baseDir, rv.ShadowCopyDirectories);
                rv.CachePath = NormalizePath(baseDir, rv.CachePath);
                rv.LicenseFile = NormalizePath(baseDir, rv.LicenseFile);
                rv.DynamicBase = NormalizePath(baseDir, rv.DynamicBase);
            }
            return rv;
        }

        private static string NormalizePath(string baseDir, string path) {
            if (string.IsNullOrEmpty(path) || Path.IsPathRooted(path)) {
                return path;
            } else if (path.Contains(";")) {
                string[] paths = path.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < paths.Length; i++) {
                    string p = paths[i];
                    if (!Path.IsPathRooted(p))
                        paths[i] = Path.GetFullPath(Path.Combine(baseDir, p));
                }
                return string.Join(";", paths);
            } else {
                return Path.GetFullPath(Path.Combine(baseDir, path));
            }
        }

        /// <summary>
        /// De-serializes an object stored in the given xml file.  IMPORTANT: all paths in the file are interpreted
        /// as being relative to the directory containing the file.
        /// </summary>
        public static AppDomainParameters Read(string parameterFile) {
            using (Stream fin = new FileStream(parameterFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (XmlReader reader = XmlReader.Create(fin)) {
                return Read(reader, Path.GetFullPath(parameterFile));
            }
        }
    }
}
