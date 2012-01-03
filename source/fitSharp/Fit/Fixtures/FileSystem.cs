// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.IO;

namespace fitSharp.Fit.Fixtures {
    public class FileSystem {

        public bool FileExists(string theFileName) {
            return File.Exists(FullName(theFileName));
        }

        public bool FileSameAs(string theFirstFile, string theSecondFile) {
            var reader = new StreamReader(FullName(theFirstFile));
            string firstContent = reader.ReadToEnd().Trim();
            reader.Close();
            reader = new StreamReader(FullName(theSecondFile));
            string secondContent = reader.ReadToEnd().Trim();
            reader.Close();
            return firstContent.Replace("\r\n","\n") == secondContent.Replace("\r\n","\n");
        }

        public bool FolderIsEmpty(string folder) {
            return Directory.GetFiles(FullName(folder)).Length == 0;
        }

        public void MakeEmptyFolder(string theFolder) {
            try {
                Directory.Delete(FullName(theFolder), true);
            }
            catch (DirectoryNotFoundException) {}
            Directory.CreateDirectory(FullName(theFolder));
        }

        public void AliasFolderAs(string theFolder, string theAlias) {
            myAliases[theAlias] = theFolder;
        }

        private string FullName(string theFileName) {
            foreach (string alias in myAliases.Keys) {
                if (theFileName.StartsWith(alias)) return myAliases[alias] + theFileName.Substring(alias.Length);
            }
            return theFileName;
        }

        private readonly Dictionary<string, string> myAliases = new Dictionary<string, string>();

    }
}
