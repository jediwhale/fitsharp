// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.IO;

namespace fit {
    public interface FolderModel {
        void MakeFile(string thePath, string theContent);
        string FileContent(string thePath);
        string[] GetFiles(string thePath);
        string[] GetFolders(string thePath);
        void CopyFile(string theInputPath, string theOutputPath);
        TextWriter MakeWriter(string thePath);
    }
}

