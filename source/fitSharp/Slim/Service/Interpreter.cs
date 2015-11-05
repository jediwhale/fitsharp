// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
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
                var instruction = messenger.Read();
                if (messenger.IsEnd) break;
                var results = processor.RunTest(() => Execute(instruction));
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
            var document = Document.Parse(instruction);
            var results = ExecuteInstructions(document.Content);
            return new Document(results).ToString();
        }

        private void AddAssemblies() {
            if (string.IsNullOrEmpty(assemblyPaths)) return;
            foreach (var path in assemblyPaths.Split(';').Where(path => path.Length != 0)) {
                processor.ApplicationUnderTest.AddAssembly(path);
            }
            assemblyPaths = null;
        }

        private static string FormatException(System.Exception e) {
            // this format is hardcoded in case there is an exception in the general formatting code
            var exception = string.Format(ComposeException.ExceptionResult, e);
            var step = string.Format("[000002:000005:error:{0:000000}:{1}:]", exception.Length, exception);
            return string.Format("[000001:{0:000000}:{1}:]", step.Length, step);
        }

        private Tree<string> ExecuteInstructions(Tree<string> instructions) {
            var results = new SlimTree();
            foreach (var statement in instructions.Branches) {
                var result = processor.Invoke(new SlimInstruction(), new MemberName(string.Empty), statement).GetValue<Tree<string>>();
                results.AddBranchValue(result);
                if (ComposeException.WasAborted(result.ValueAt(1))) break;
            }
            return results;
        }
    }
}
