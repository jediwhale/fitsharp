// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fitSharp.IO;

namespace fitSharp.Fit.Fixtures {
    public class FileSystem {

        public bool FileExists(string theFileName) {
            return File.Exists(FullName(theFileName));
        }

        public bool FileSameAs(PathId theFirstFile, PathId theSecondFile) {
            return FileSame(theFirstFile, theSecondFile, s => s.Replace("\r\n", "\n"));
        }

        public bool FileWithPathsSameAs(PathId theFirstFile, PathId theSecondFile) {
            return FileSame(theFirstFile, theSecondFile, s => PathId.AsOS(s.Replace("\r\n", "\n")));
        }
        
        bool FileSame(PathId theFirstFile, PathId theSecondFile, Func<string, string> replace) {
            var reader = new StreamReader(FullName(theFirstFile.Path));
            string firstContent = reader.ReadToEnd().Trim();
            reader.Close();
            reader = new StreamReader(FullName(theSecondFile.Path));
            string secondContent = reader.ReadToEnd().Trim();
            reader.Close();
            return replace(firstContent) == replace(secondContent);
        }

        public bool FolderIsEmpty(PathId folder) {
            return Directory.GetFiles(FullName(folder.Path)).Length == 0;
        }

        public void MakeEmptyFolder(PathId theFolder) {
            try {
                Directory.Delete(FullName(theFolder.Path), true);
            }
            catch (DirectoryNotFoundException) {}
            Directory.CreateDirectory(FullName(theFolder.Path));
        }

        public FileSystemFile MakeEmptyFile(PathId fileName) {
            return new FileSystemFile(fileName.Path);
        }

        public void AliasFolderAs(PathId theFolder, string theAlias) {
            myAliases[theAlias] = theFolder.Path;
        }

        string FullName(string theFileName) {
            foreach (var alias in myAliases.Keys.Where(theFileName.StartsWith)) {
                return myAliases[alias] + theFileName.Substring(alias.Length);
            }

            return theFileName;
        }

        readonly Dictionary<string, string> myAliases = new Dictionary<string, string>();

    }

    public class FileSystemFile {
        public FileSystemFile(string name) {
            this.name = name;
        }

        public void WriteLine(string line) {
            using (var writer = File.AppendText(name)) {
                writer.WriteLine(line);
            }
        }

        public void WriteLineWithPath(string line) {
            using (var writer = File.AppendText(name)) {
                writer.WriteLine(PathId.AsOS(line));
            }
        }

        readonly string name;
    }
}
