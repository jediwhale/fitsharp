// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using System.Text.RegularExpressions;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class ParseSymbol: Operator<string>, ParseOperator<string> {
        private static readonly Regex symbolPattern = new Regex("\\$([a-zA-Z]\\w*)");

        public bool CanParse(Type type, TypedValue instance, Tree<string> parameters) {
            if (string.IsNullOrEmpty(parameters.Value)) return false;
            string decodedInput = ReplaceSymbols(parameters.Value);
            return parameters.Value != decodedInput;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<string> parameters) {
            string decodedInput = ReplaceSymbols(parameters.Value);
            return Processor.Parse(type, decodedInput);
        }

        private string ReplaceSymbols(string input) {
            var result = new StringBuilder();
            int lastMatch = 0;
            for (Match symbolMatch = symbolPattern.Match(input); symbolMatch.Success; symbolMatch = symbolMatch.NextMatch()) {
                string symbolName = symbolMatch.Groups[1].Value;
                if (symbolMatch.Index > lastMatch) result.Append(input.Substring(lastMatch, symbolMatch.Index - lastMatch));
                result.Append(Processor.Contains(new Symbol(symbolName)) ? Processor.Load(new Symbol(symbolName)).Instance : symbolName);
                lastMatch = symbolMatch.Index + symbolMatch.Length;
            }
            if (lastMatch < input.Length) result.Append(input.Substring(lastMatch, input.Length - lastMatch));
            return result.ToString();
        }
    }
}