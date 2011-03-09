using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Application
{
    public class ArgumentParser
    {
        Dictionary<string, Action<string>> argumentHandlers;
        Dictionary<string, Action> switchHandlers;

        public ArgumentParser()
        {
            argumentHandlers = new Dictionary<string, Action<string>>();
            switchHandlers = new Dictionary<string, Action>();
        }

        public void AddArgumentHandler(string argument, Action<string> handler) {
            argumentHandlers.Add(argument, handler);
        }

        public void AddSwitchHandler(string @switch, Action handler)
        {
            switchHandlers.Add(@switch, handler);
        }

        public void Parse(string[] commandLineArguments)
        {            
            for (int i = 0; i < commandLineArguments.Length; i++)
            {
                if (commandLineArguments[i].StartsWith("-") && i == commandLineArguments.Length-1)
                {
                    string switchName = commandLineArguments[i].Substring(1);
                    InvokeSwitchHandler(switchName);
                }

                else if (commandLineArguments[i].StartsWith("-") && commandLineArguments[i + 1].StartsWith("-"))
                {
                    string switchName = commandLineArguments[i].Substring(1);
                    InvokeSwitchHandler(switchName);
                }
                else if (commandLineArguments[i].StartsWith("-"))
                {
                    string switchName = commandLineArguments[i].Substring(1);
                    InvokeArgumentHandler(switchName, commandLineArguments[i + 1]);
                }
            }
        }

        private void InvokeSwitchHandler(string switchName)
        {
            Action handler = null;
            if (switchHandlers.TryGetValue(switchName, out handler))
                handler();
        }

        private void InvokeArgumentHandler(string switchName, string arguments)
        {
            Action<string> handler = null;
            if (argumentHandlers.TryGetValue(switchName, out handler))
                handler(arguments);
            //TODO: look into this Kim
        }
    }
}
