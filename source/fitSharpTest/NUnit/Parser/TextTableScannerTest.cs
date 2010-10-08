// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Parser;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture] public class TextTableScannerTest {
        [Test] public void ScansEmptyInputAsEndToken() {
            AssertRawScan(string.Empty, "End");
        }

        static void AssertRawScan(string input, string expected) {
            var scanner = new TextTableScanner(input, c => c.IsLetterOrWhitespace);
            var result = new StringBuilder();
            foreach (Token token in scanner.Tokens) {
                if (result.Length > 0) result.Append(",");
                result.AppendFormat("{0}", token.Type);
                if (token.Content.Length > 0) result.AppendFormat("={0}", token.Content);
            }
            Assert.AreEqual(expected, result.ToString());
        }

        [Test] public void ScansPrefixAsLeader() {
            AssertRawScan("more test@stuff", "Leader=more test@,Word=stuff,End");
        }

        [Test] public void ScansTrailerAsEnd() {
            AssertRawScan("test@more@test stuff", "Leader=test@,Word=more,End=@test stuff");
        }

        [Test] public void ScansMultipleTests() {
            AssertRawScan("test@more@test test@stuff", "Leader=test@,Word=more,Leader=@test test@,Word=stuff,End");
        }

        [Test] public void ScansInputWithoutPrefixAsEnd() {
            AssertRawScan("stuff", "End=stuff");
        }

        [Test] public void ScansTextAsWord() {
            AssertScan("more stuff", "Word=more stuff");
        }

        static void AssertScan(string input, string expected) {
            AssertRawScan("test@" + input, "Leader=test@," + expected + ",End");
        }
 
        [Test] public void ScansSeparatedTextAsWords() {
            AssertScan("more|stuff", "Word=more,Word=stuff");
        }

        [Test] public void IgnoresWhitespaceInSeparatedText() {
            AssertScan("more | stuff", "Word=more,Word=stuff");
        }

        [Test] public void ScansDelimitedTextAsWords() {
            AssertScan("'more'\"stuff\"", "Word=more,Word=stuff");
        }

        [Test] public void ScansEmptyDelimitedTextAsWord() {
            AssertScan("''\"stuff\"", "Word,Word=stuff");
        }

        [Test] public void RetainsWhitespaceInDelimitedText() {
            AssertScan("'more '\" stuff\"", "Word=more ,Word= stuff");
        }

        [Test] public void RetainsSpecialCharactersInDelimitedText() {
            AssertScan("'more|stuff'", "Word=more|stuff");
        }

        [Test] public void ScansFreeAndDelimitedTextAsWords() {
            AssertScan("more'stuff'", "Word=more,Word=stuff");
        }

        [Test] public void IgnoresWhitespaceBetweenFreeAndDelimitedText() {
            AssertScan("more 'stuff'", "Word=more,Word=stuff");
        }

        [Test] public void ScansDelimitedAndFreeTextAsWords() {
            AssertScan("'more'stuff", "Word=more,Word=stuff");
        }

        [Test] public void IgnoresWhitespaceBetweenDelimitedText() {
            AssertScan("'more'  \"stuff\"", "Word=more,Word=stuff");
        }

        [Test] public void ScansNewlineAsNewline() {
            AssertScan("more\nstuff", "Word=more,Newline,Word=stuff");
        }

        [Test] public void ScansBreakAsNewline() {
            AssertScan("more<br>stuff", "Word=more,Newline,Word=stuff");
        }

        [Test] public void IgnoresEscapedQuote() {
            AssertScan("I\\'m\nstuff", "Word=I'm,Newline,Word=stuff");
        }

    }
}
