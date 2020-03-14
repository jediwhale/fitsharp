﻿// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.IO;
using fitSharp.Machine.Application;

namespace fitSharp.Runner {
    public class Program {
        public static int Main(string[] arguments) {
            return new Shell(new ConsoleReporter(), new ShellArguments(new FileSystemModel(), arguments)).Run();
        }
    }
}
