// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.IO {
    public interface PageSource {
        string GetPageContent(Path pageName);
        Path MakePath(string pageName);
    }

    public static class PageSourceExtension {
        public static string GetPageContent(this PageSource pageSource, string pageName) {
            return pageSource.GetPageContent(pageSource.MakePath(pageName));
        }
    }

    public interface Path {
        Path WithSubPath(Path subPath);
    }

    public class FilePath: Path {
        public FilePath(string filePath) { this.filePath = filePath; }
        public override string ToString() { return filePath; }

        public Path WithSubPath(Path subPath) {
            return new FilePath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), subPath.ToString()));
        }

        readonly string filePath;
    }

    public class DirectoryPath: Path {
        public DirectoryPath(string directoryPath) { this.directoryPath = directoryPath; }
        public override string ToString() { return directoryPath; }

        public Path WithSubPath(Path subPath) {
            return new FilePath(System.IO.Path.Combine(directoryPath, subPath.ToString()));
        }

        readonly string directoryPath;
    }
}
