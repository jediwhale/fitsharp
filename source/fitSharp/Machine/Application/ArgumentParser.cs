// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Machine.Application {
    public class ArgumentParser {

        public static IEnumerable<string> Extras(IList<string> arguments, IList<string> switches) {
            for (var i = 0; i < arguments.Count; i++) {
                if (arguments[i].StartsWith("-") && switches.Contains(arguments[i].Substring(1))) {
                    if (i < arguments.Count - 1 && !arguments[i + 1].StartsWith("-")) {
                        i++;
                    }
                }
                else {
                    yield return arguments[i];
                }
            }
        }

        public void AddArgumentHandler(string @switch, Action<string> action) {
            argumentHandlers.Add(@switch, action);
        }

        public void AddSwitchHandler(string @switch, Action action) {
            switchHandlers.Add(@switch, action);
        }

        public void Parse(IList<string> commandLineArguments) {
            for (var i = 0; i < commandLineArguments.Count; i++) {
                if (commandLineArguments[i].StartsWith("-")) {
                    if (i == commandLineArguments.Count - 1 || commandLineArguments[i + 1].StartsWith("-")) {
                        InvokeSwitchHandler(commandLineArguments[i].Substring(1));
                    }
                    else {
                        InvokeArgumentHandler(commandLineArguments[i].Substring(1), commandLineArguments[i + 1]);
                        i++;
                    }
                }
            }
        }

        void InvokeArgumentHandler(string @switch, string argumentValue) {
            if (argumentHandlers.ContainsKey(@switch)) {
                argumentHandlers[@switch](argumentValue);
            }
        }

        void InvokeSwitchHandler(string @switch) {
            if (switchHandlers.ContainsKey(@switch)) {
                switchHandlers[@switch]();
            }
        }

        readonly Dictionary<string, Action<string>> argumentHandlers = new Dictionary<string, Action<string>>();
        readonly Dictionary<string, Action> switchHandlers = new Dictionary<string, Action>();
    }
}
