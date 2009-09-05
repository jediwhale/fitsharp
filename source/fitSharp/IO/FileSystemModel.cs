// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;
using fitSharp.Machine.Application;

namespace fitSharp.IO {
    public class FileSystemModel: FolderModel {
        
        private const int ourDefaultCodePage = 1252;

        public FileSystemModel() {
            myCodePage = ourDefaultCodePage;
            if (Context.Configuration == null) return;
            string codePage = Context.Configuration.GetItem<Settings>().CodePage;
            if (codePage != null) int.TryParse(codePage, out myCodePage);
        }

        public void MakeFile(string thePath, string theContent) {
            if (!Directory.Exists(Path.GetDirectoryName(thePath))) {
                Directory.CreateDirectory(Path.GetDirectoryName(thePath));
            }
            TextWriter writer = new StreamWriter(thePath, false, Encoding.GetEncoding(myCodePage));
            writer.Write(theContent);
            writer.Close();
        }

        public TextWriter MakeWriter(string thePath) {
            return new StreamWriter(thePath);
        }

        public string FileContent(string thePath) {
            
            StreamReader reader;
            try {
                var file = new FileStream(thePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new StreamReader(file, Encoding.GetEncoding(myCodePage));
            }
            catch (FileNotFoundException) {
                return null;
            }
            string result = reader.ReadToEnd();
            reader.Close();
            return result;
        }

        public string[] GetFiles(string thePath) {
            return Directory.Exists(thePath)
                       ? Directory.GetFiles(thePath)
                       : (File.Exists(thePath) ? new[] {thePath} : new string[] {});
        }

        public string[] GetFolders(string thePath) {
            return !Directory.Exists(thePath) ? new string[] {} : Directory.GetDirectories(thePath);
        }

        public void CopyFile(string theInputPath, string theOutputPath) {
            try {
                File.Delete(theOutputPath);
            }
            catch (DirectoryNotFoundException) {
                Directory.CreateDirectory(Path.GetDirectoryName(theOutputPath));
            }
            File.Copy(theInputPath, theOutputPath);
        }
        
        public void WriteToConsole(string theMessage) {
            Console.Write(theMessage);
        }

        private readonly int myCodePage;
    }
}
