// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using System.IO;

namespace fit {
    public class FileSystem {

        public bool FileExists(string theFileName) {
            return File.Exists(FullName(theFileName));
        }

        public bool FileSameAs(string theFirstFile, string theSecondFile) {
            StreamReader reader = new StreamReader(FullName(theFirstFile));
            string firstContent = reader.ReadToEnd().Trim();
            reader.Close();
            reader = new StreamReader(FullName(theSecondFile));
            string secondContent = reader.ReadToEnd().Trim();
            reader.Close();
            return firstContent.Replace("\r\n","\n") == secondContent.Replace("\r\n","\n");
        }

        public bool FolderIsEmpty(string folder) {
            return (Directory.GetFiles(FullName(folder)).Length == 0);
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

        private Dictionary<string, string> myAliases = new Dictionary<string, string>();

    }
}
