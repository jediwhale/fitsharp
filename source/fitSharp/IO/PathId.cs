// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Runtime.InteropServices;

namespace fitSharp.IO {
    public class PathId {
        public static PathId Parse(string input) { return new PathId(input); }

        public static string AsOS(string input) {
            return input
                .Replace('\\', System.IO.Path.DirectorySeparatorChar)
                .Replace("$OS$", osName);
        }

        public static string AsWindows(string input) {
            return input
                .Replace(System.IO.Path.DirectorySeparatorChar, '\\');
        }
        
        PathId(string id) { this.id = id; }

        public string Path => AsOS(id);

        
        static readonly string osName = 
            #if NETCOREAPP
                (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" : "linux")
            #else
                "windows"
            #endif
            ;
        readonly string id;
    }
}