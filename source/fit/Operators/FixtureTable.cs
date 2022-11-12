// Copyright � 2022 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Linq;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class FixtureTable {

        readonly Parse myTable;

        public FixtureTable(Parse theTable) {
            myTable = theTable;
        }

        public string Differences(FixtureTable theExpected) {
            return Differences(myTable, theExpected.myTable);
        }

        static string Differences(Parse theActual, Parse theExpected) {
            if (theActual == null) {
                return (theExpected != null ? FormatNodeDifference(theActual, theExpected) : string.Empty);
            }
            if (theExpected == null) {
                return FormatNodeDifference(theActual, theExpected);
            }
            var actualString = theActual.ToString().Replace("\n", string.Empty).Replace("\r", string.Empty);
            var expectedString = theExpected.ToString().Replace("\n", string.Empty).Replace("\r", string.Empty);
            if (actualString == expectedString) return string.Empty;

            var expected = new Expected(theExpected);
            if (theActual.Tag != expected.Node.Tag) {
                return FormatNodeDifference(theActual, expected.Node);
            }
            string result = BodyDifferences(theActual.Body,  expected.Node.Body);
            if (result.Length > 0) {
                return string.Format("in {0} body, {1}", theActual.Tag, result);
            }
            result = Differences(theActual.Parts,  theExpected.Parts);
            if (result.Length > 0) {
                return string.Format("in {0}, {1}", theActual.Tag, result);
            }
            return Differences(theActual.More, theExpected.More);
        }

        static string FormatNodeDifference(Parse actualNode, Parse expectedNode) {
            return FormatDifference(TagString(actualNode),  TagString(expectedNode));
        }

        static string FormatDifference(string actual, string expected) {
            return string.Format("expected: {1}, was {0}", FormatText(actual), FormatText(expected));
        }

        static string FormatText(string text) {
            return text == null ? "null" : string.Format("'{0}'", text);
        }

        static string TagString(Parse node) {
            return node == null ? null : node.Tag;
        }

        static string BodyDifferences(string theActual, string theExpected) {
            if (theExpected != null && theExpected == "IGNORE") return string.Empty;
            if (theActual == null) {
                return (!string.IsNullOrEmpty(theExpected) ? FormatDifference(null, theExpected) : string.Empty);
            }
            if (theExpected == null) {
                return (theActual.Length > 0 ? FormatDifference(theActual, null) : string.Empty);
            }
            string[] actual = SplitBody(theActual);
            string [] expected = SplitBody(theExpected);
            if (actual[0].Trim() != expected[0].Trim()) {
                return FormatDifference(actual[0], expected[0]);
            }
            if (actual[1] == null) {
                return (expected[1] != null ? FormatDifference(null, expected[1]) : string.Empty);
            }
            if (expected[1] == null) {
                return (actual[1] != null ? FormatDifference(actual[1], null) : string.Empty);
            }
            return ContainsText(actual[1], expected[1]) ? string.Empty : FormatDifference(actual[1], expected[1]);
        }

        static bool ContainsText(string actual, string expected) {
            return expected.Split(new [] {".."}, StringSplitOptions.None)
                .All(e => actual.Replace("\r\n", "\n").IndexOf(e.Replace("\r\n", "\n"), StringComparison.OrdinalIgnoreCase) >= 0);
        }

        static string[] SplitBody(string theSource) {
            var result = new string[2];
            int marker = theSource.IndexOf("fit_stacktrace");
            if (marker < 0) {
                result[0] = theSource;
            }
            else {
                result[0] = theSource.Substring(0, theSource.IndexOf('<'));
                int start = theSource.IndexOf('>', marker) + 1;
                int end = theSource.IndexOf('<', start);
                result[1] = theSource.Substring(start, end - start);
            }
            return result;
        }

        class Expected {
            public Parse Node { get; private set;}
            readonly Parse originalNode;

            public Expected(Parse node) {
                originalNode = node;
                Node = node.Copy();
                if (HasKeyword("right")) {
                    Node.SetAttribute(CellAttribute.Status, TestStatus.Right);
                    if (BodyStartsWith("right")) Node.SetBody(Node.Body.Substring(7));
                }
                else if (HasKeyword("wrong")) {
                    Node.SetAttribute(CellAttribute.Status, TestStatus.Wrong);
                    if (BodyStartsWith("wrong")) Node.SetBody(Node.Body.Substring(7));
                    else Node.SetBody("IGNORE");
                }
                if (Node.Body.StartsWith("/")) Node.SetBody(Node.Body.Substring(1));
            }

            bool HasKeyword(string keyword) {
                return BodyStartsWith(keyword) || EmbeddedLeaderStartsWith(keyword);
            }

            bool EmbeddedLeaderStartsWith(string keyword) {
                return (originalNode.Parts != null && originalNode.Parts.Leader.StartsWith(KeywordMarker(keyword)));
            }

            bool BodyStartsWith(string keyword) {
                return originalNode.Body.StartsWith(KeywordMarker(keyword));
            }

            static string KeywordMarker(string keyword) {
                return string.Format("[{0}]", keyword);
            }
        }
    }
}