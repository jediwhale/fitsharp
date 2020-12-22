// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace fitSharp.Machine.Model {
    public static class TargetFramework {
        public static string Location(Assembly assembly) {
            return
                #if NET5_0
                    OperatingSystem.IsWindows() ? assembly.Location.Replace("\\", "/") : assembly.Location
                #else
                    assembly.CodeBase
                #endif
                ;
        }

        public static string FileExtension =>
            #if NET5_0
                OperatingSystem.IsWindows() ? "net5" : "linux"
            #else
                #if NETCOREAPP
                    "netcore"
                #else
                    "netfx"
                #endif
            #endif
            ;
        
        public static bool IsWindows =>
            #if NET5_0
                OperatingSystem.IsWindows()
            #else
                #if NETCOREAPP
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                #else
                    true
                #endif
            #endif
            ;
    }
}