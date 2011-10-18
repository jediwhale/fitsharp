// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class SlimTreeTest {
        [Test] public void SingleItemListIsParsed() {
            AssertParsed("[a]", new [] {"a"});
        }

        [Test] public void EmptyListIsParsed() {
            AssertParsed("[]", new string[] {});
        }

        [Test] public void MultipleItemListIsParsed() {
            AssertParsed("[a, b, c]", new [] {"a", "b", "c"});
        }

        [Test] public void EmptyItemListIsParsed() {
            AssertParsed("[a, , c]", new [] {"a", "", "c"});
        }

        [Test] public void MissingOpenBracketIsError() {
            AssertParseError("a]");
        }

        [Test] public void MissingCloseBracketIsError() {
            AssertParseError("[a");
        }

        [Test] public void NestedSingleItemListIsParsed() {
            AssertParsed("[a, [b], c]", new object[] {"a", new [] {"b"}, "c"});
        }

        [Test] public void NestedMultipleItemListIsParsed() {
            AssertParsed("[a, [b, c], d]", new object[] {"a", new [] {"b", "c"}, "d"});
        }

        [Test] public void NestedFinalItemListIsParsed() {
            AssertParsed("[a, b, [c, d]]", new object[] {"a", "b", new [] {"c", "d"}});
        }

        [Test] public void NestingWithoutClosingBracketIsIgnored() {
            AssertParsed("[a, [b, c]", new object[] {"a", "[b", "c"});
        }

        [Test] public void NestingWithoutCommaIsIgnored() {
            AssertParsed("[a, [b] c]", new object[] {"a", "[b] c",});
        }

        static void AssertParsed(string input, IList<object> results) {
            ValidateTree(SlimTree.Parse(input), results);
        }

        static void ValidateTree(Tree<string> result, IList<object> results) {
            Assert.AreEqual(results.Count, result.Branches.Count);
            for (var i = 0; i < results.Count; i++) {
                var itemList = results[i] as IList<object>;
                if (itemList == null) Assert.AreEqual(results[i], result.Branches[i].Value);
                else ValidateTree(result.Branches[i], itemList);
            }
        }

        static void AssertParseError(string input) {
            try {
                SlimTree.Parse(input);
            }
            catch (FormatException) {
                Assert.Pass();
            }
            Assert.Fail();
        }
    }
}
