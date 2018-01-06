// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;

namespace fitSharp.Parser {
    public class ExcelCell {
        public ExcelCell(string address, string content) {
            this.address = address;
            Content = content;
        }

        public string Content { get; }

        public int Column =>
            address
                .TakeWhile(IsLetter)
                .Aggregate(-1, (current, character) => (current + 1) * 26 + character - 'A');

        public int Row => int.Parse(address.Substring(address.Count(IsLetter))) - 1;

        static bool IsLetter(char character) {
            return character >= 'A' && character <= 'Z';
        }

        readonly string address;
    }
}
