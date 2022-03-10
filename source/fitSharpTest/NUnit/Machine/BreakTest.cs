// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture]
    public class BreakTest {
        [Test]
        public void EmptyInputCallsNothing() {
            AssertBreak("");
        }

        [Test]
        public void SingleInputCallsEachOnce() {
            AssertBreak("start:akey;item:akey,avalue;end:akey;",
                Tuple.Create("akey", "avalue"));
        }

        [Test]
        public void SingleKeyMultipleItemsCallsKeyOnce() {
            AssertBreak("start:akey;item:akey,avalue;item:akey,bvalue;end:akey;",
                Tuple.Create("akey", "avalue"), Tuple.Create("akey", "bvalue"));
        }

        [Test]
        public void MultipleKeyMultipleItemsCallsEachOnce() {
            AssertBreak("start:akey;item:akey,avalue;end:akey;start:bkey;item:bkey,bvalue;end:bkey;",
                Tuple.Create("akey", "avalue"), Tuple.Create("bkey", "bvalue"));
        }

        void AssertBreak(string expected, params Tuple<string, string>[] input) {
            result = string.Empty;
            new Break<string, string>(Start, Item, End).Process(input);
            Assert.AreEqual(expected, result);
        }

        void Item(string key, string value) {
            result = $"{result}item:{key},{value};";
        }

        void Start(string start) {
            result = $"{result}start:{start};";
        }

        void End(string start) {
            result = $"{result}end:{start};";
        }

        string result;
    }
}