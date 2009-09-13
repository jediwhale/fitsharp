// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;

namespace fitSharp.Slim.Service {
    public class Interpreter {
        private readonly Messenger messenger;
        private string assemblyPaths;
        private readonly Service processor;

        public Interpreter(Messenger messenger, string assemblyPaths, Service processor) {
            this.messenger = messenger;
            this.assemblyPaths = assemblyPaths;
            this.processor = processor;
        }

        public void ProcessInstructions() {
            while (true) {
                string instruction = messenger.Read();
                if (messenger.IsEnd) break;
                string results = Execute(instruction);
                messenger.Write(results);
            }
        }

        private string Execute(string instruction) {
            try {
                AddAssemblies();
                return ExecuteInstruction(instruction);
            }
            catch (System.Exception e) {
                return FormatException(e);
            }
        }

        private string ExecuteInstruction(string instruction) {
            Document document = Document.Parse(instruction);
            Tree<string> results = ExecuteInstructions(document.Content);
            return new Document(results).ToString();
        }

        private void AddAssemblies() {
            if (string.IsNullOrEmpty(assemblyPaths)) return;
            foreach (string path in assemblyPaths.Split(';')) {
                if (path.Length == 0) continue;
                processor.ApplicationUnderTest.AddAssembly(path);
            }
            assemblyPaths = null;
        }

        private static string FormatException(System.Exception e) {
            // this format is hardcoded in case there is an exception in the general formatting code
            string exception = string.Format(ComposeException.ExceptionResult, e);
            string step = string.Format("[000002:000005:error:{0:000000}:{1}:]", exception.Length, exception);
            return string.Format("[000001:{0:000000}:{1}:]", step.Length, step);
        }

        private Tree<string> ExecuteInstructions(Tree<string> instructions) {
            var results = new TreeList<string>();
            foreach (Tree<string> statement in instructions.Branches) {
                var result = (Tree<string>) processor.Execute(TypedValue.Void, statement).Value;
                results.AddBranchValue(result);
                if (ComposeException.WasAborted(result.Branches[1].Value)) break;
            }
            return results;
        }
    }
}
