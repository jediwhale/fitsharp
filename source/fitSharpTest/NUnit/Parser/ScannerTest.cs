﻿// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Parser;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture]
    public class ScannerTest {

        [Test]
        public void FindsDelimitedText() {
            Scan("basic [text] to be scanned");
            AssertLeader("basic ");
            AssertBody("text");
            ClassicAssert.AreEqual("[text]", scanner.Element.ToString());
        }

        [Test]
        public void FindsNextDelimitedText() {
            Scan("basic [text] to [be] scanned");
            Scan();
            AssertLeader(" to ");
            AssertBody("be");
        }

        [Test]
        public void FindsTrailingText() {
            Scan("basic [text] to be scanned");
            Scan();
            AssertLeader(" to be scanned");
            AssertBody(string.Empty);
        }

        [Test]
        public void EmptyInputReturnsEmptyResults() {
            AssertEmpty(string.Empty);
        }

        [Test]
        public void MissingFirstTokenReturnsEmptyBody() {
            AssertEmpty("no first]");
        }

        [Test]
        public void MissingSecondTokenReturnsEmptyBody() {
            AssertEmpty("no [second");
        }

        [Test]
        public void OutOfOrderTokensReturnsEmptyBody() {
            AssertEmpty("out ]of[ order");
        }

        [Test]
        public void AdjacentTokensAreIgnoredy() {
            AssertEmpty("scan [] this");
        }

        [Test]
        public void EmptyLeaderIsOK() {
            Scan("[text] to be scanned");
            AssertLeader(string.Empty);
            AssertBody("text");
        }

        [Test]
        public void BodyCanBeFiltered() {
            scanner = new Scanner("[not me] but [me]");
            scanner.FindTokenPair("[", "]", s => !s.Equals("not me"));
            AssertLeader("[not me] but ");
            AssertBody("me");
        }

        [Test]
        public void TokensAreCaseInsenstive() {
            scanner = new Scanner("some <TeXt for YoU>");
            ScanWith("<Text", "YoU>");
            AssertBody(" for ");
        }

        void AssertEmpty(string input) {
            Scan(input);
            AssertLeader(input);
            AssertBody(string.Empty);
        }

        void AssertBody(string expected) {
            ClassicAssert.AreEqual(expected, scanner.Body.ToString());
        }

        void AssertLeader(string expected) {
            ClassicAssert.AreEqual(expected, scanner.Leader.ToString());
        }

        void Scan(string input) {
            scanner = new Scanner(input);
            Scan();
        }

        void Scan() {
            ScanWith("[", "]");
        }

        void ScanWith(string firstToken, string secondToken) {
            scanner.FindTokenPair(firstToken, secondToken);
        }

        Scanner scanner;
    }
}
