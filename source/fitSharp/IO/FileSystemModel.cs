// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;

namespace fitSharp.IO {
    public class FileSystemModel: FolderModel {
        
        private readonly Encoding encoding;

        public FileSystemModel() {
            encoding = Encoding.UTF8;
        }

        public FileSystemModel(int codePage) {
            encoding = Encoding.GetEncoding(codePage);
        }

        public void MakeFile(string thePath, string theContent) {
            if (!Directory.Exists(Path.GetDirectoryName(thePath))) {
                Directory.CreateDirectory(Path.GetDirectoryName(thePath));
            }
            TextWriter writer = new StreamWriter(thePath, false, encoding);
            writer.Write(theContent);
            writer.Close();
        }

        public TextWriter MakeWriter(string thePath) {
            return new StreamWriter(thePath);
        }

        public string FileContent(string thePath)
        {
            try {
                using (FileStream file = new FileStream(thePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (StreamReader reader = new StreamReader(file, encoding)) {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (FileNotFoundException) {
                // File does not exist, return null
            }
            catch (DirectoryNotFoundException) {
                // File's parent directory does not exist, return null
            }

            return null;
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
    }
}
