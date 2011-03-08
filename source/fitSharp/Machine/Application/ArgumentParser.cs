using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application
{
    public class ArgumentParser {
        Dictionary<string, Action<string>> argumentHandlers;

        public ArgumentParser() {
            argumentHandlers = new Dictionary<string, Action<string>>();
        }

        public void AddArgumentHandler(string name, Action<string> handler) {
            argumentHandlers.Add(name, handler);
        }

        public void Parse(string[] commandLineArguments) {            
            for (int i = 0; i < commandLineArguments.Length; i++) {
                if (IsSwitch(commandLineArguments[i])) {
                    string name = commandLineArguments[i].Substring(1);
                    
                    //Order on if statement is important. 
                    if (i == commandLineArguments.Length - 1) {   
                        InvokeArgumentHandler(name);
                    }
                    else if (IsSwitch(commandLineArguments[i + 1])) {
                        InvokeArgumentHandler(name);
                    }
                    else {
                        InvokeArgumentHandler(name, commandLineArguments[i + 1]);
                    }
                }
            }
        }

        private static bool IsSwitch(string argument) {
            return argument.StartsWith("-");
        }

        private void InvokeArgumentHandler(string name) {
            InvokeArgumentHandler(name, string.Empty);
        }

        private void InvokeArgumentHandler(string name, string value) {
            Action<string> handler = null;
            if (argumentHandlers.TryGetValue(name, out handler))
                handler.Invoke(value);
        }
    }
}
