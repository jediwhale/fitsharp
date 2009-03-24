// FitNesse.NET
// Copyright © 2007, 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using System.Text;
using fitSharp.Machine.Application;

namespace fit {
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
                FileStream file = new FileStream(thePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
            if (Directory.Exists(thePath)) return Directory.GetFiles(thePath);
            if (File.Exists(thePath)) return new string[] {thePath};
            return new string[] {};
        }

        public string[] GetFolders(string thePath) {
            if (Directory.Exists(thePath)) return Directory.GetDirectories(thePath);
            return new string[] {};
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

        private int myCodePage;
    }
}
