// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.IO;
using fitSharp.IO;

namespace fitSharp.Test.Double
{
    public class FolderTestModel: FolderModel, ProgressReporter {
        public FolderTestModel() {
            myFiles = new Hashtable();
        }

        public void MakeFile(string thePath, string theContent) {
            myFiles[thePath] = theContent;
        }

        public TextWriter MakeWriter(string thePath) {
            TextWriter writer = new StringWriter();
            myFiles[thePath] = writer;
            return writer;
        }

        public string FileContent(string thePath) {
            return (myFiles.ContainsKey(thePath) ? myFiles[thePath].ToString() : null);
        }

        public bool FileExists(string thePath) {
            return myFiles.ContainsKey(thePath);
        }

        public string[] GetFiles(string thePath) {
            if (thePath.Contains(".")) return new [] {thePath};
            var result = new ArrayList();
            foreach (string file in myFiles.Keys) {
                if (file.StartsWith(thePath + "\\") && file.Substring(thePath.Length + 1).IndexOf("\\") < 0) result.Add(file);
            }
            var files = (string[])result.ToArray(typeof(string));
            Array.Sort(files);
            return files;
        }

        public string[] GetFolders(string thePath) {
            var result = new ArrayList();
            foreach (string file in myFiles.Keys) {
                if (!file.StartsWith(thePath + "\\")) continue;
                int length = file.LastIndexOf("\\");
                if (length <= thePath.Length) continue;
                string folder = file.Substring(0, length);
                if (!result.Contains(folder)) result.Add(folder);
            }
            return (string[])result.ToArray(typeof(string));
        }

        public void CopyFile(string theInputPath, string theOutputPath) {
            MakeFile(theOutputPath, FileContent(theInputPath));
        }
        
        public void Write(string theMessage) {}

        private readonly Hashtable myFiles;
    }
}