// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;

namespace fitSharp.IO {
    public interface FolderModel: PageSource {
        void MakeFile(string thePath, string theContent);
        string[] GetFiles(string thePath);
        string[] GetFolders(string thePath);
        void CopyFile(string theInputPath, string theOutputPath);
        TextWriter MakeWriter(string thePath);
    }
}
