// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using NUnit.Framework;
using fitSharp.Fit.Runner;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {

    [TestFixture]
    public class TagFilterTest {

        [Test]
        public void ConstructorShouldNotAllowEmptyFilter() {
            ClassicAssert.Throws<ArgumentException>(() => new TagFilter(""));
        }

        [Test]
        public void ConstructorShouldNotAllowNullFilter() {
            ClassicAssert.Throws<ArgumentException>(() => new TagFilter(null));
        }

        [Test]
        public void MatchItemInTags() {
            ClassicAssert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", "A,B"));
        }
        
        [Test]
        public void MatchItemInTagsWithSpaces() {
            ClassicAssert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", " A, B  "));            
        }
        [Test]
        public void MatchTagWithOnlyOneTagItem() {
            ClassicAssert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: B</div>", "B"));            
        }
        [Test]
        public void ShouldNotMatchItemNotInFilter() {
            ClassicAssert.IsFalse(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: B</div>", "B-L"));
        }

        [Test]
        public void CaseInsensitiveIdMatch() {
            ClassicAssert.IsTrue(MatchesFilter(@"<div id=""FiT_TagS"">Tags: A</div>", "A"));
        }

        [Test]
        public void CaseInsensitiveTagMatch() {
            ClassicAssert.IsTrue(MatchesFilter(@"<p id=""fit_tags"">Tags: A</p>", "a"));
        }

        [Test]
        public void ShouldNotMatchIfNoIdAttribute() {
            ClassicAssert.IsFalse(MatchesFilter(@"<div ""fit_tags"">Tags: A</div>", "A"));
        }

        [Test]
        public void ShouldNotMatchContentWithoutTags() {
          ClassicAssert.IsFalse(MatchesFilter(@"<p>Content</p>", "A"));
        }

        [Test]
        public void ShouldNotMatchItemUnlessIdAttributeIs_fit_tags() {
            ClassicAssert.IsFalse(MatchesFilter(@"<div id=""fit_"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", "A, B"));
            ClassicAssert.IsFalse(MatchesFilter(@"<div id=""fit_Tags_testing"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", "A, B"));
        }

        [Test]
        public void ShouldMatchWithoutLeadingTagsLabel() {
            ClassicAssert.IsTrue(MatchesFilter(@"<div id=""fit_tags"">A,B</div>", "A, B"));
        }

        [Test]
        public void ShouldMatchWithParagraphElement() {
            ClassicAssert.IsTrue(MatchesFilter(@"<p id=""fit_tags"">Tags: A,B</p>", "A, B"));
        }

        [Test]
        public void ShouldNotMatchTagSpecificationWithinComment() {
            ClassicAssert.IsFalse(MatchesFilter(@"<!-- p id=""fit_tags"">Tags: A,B</p -->", "A, B"));
        }

        [Test]
        public void ShouldNotMatchPartsOfTagNames() {
            ClassicAssert.IsFalse(MatchesFilter(@"<p id=""fit_tags"">Tags: B-L</p>", "B"));
        }

        [Test]
        public void AllowsFitTagsWithSingleQuotes() {
            ClassicAssert.IsTrue(MatchesFilter(@"<p id='fit_tags'>Tags: A</p>", "A"));
        }

        [Test]
        public void AllowsFitTagsWithoutQuotes() {
            ClassicAssert.IsTrue(MatchesFilter(@"<p id=fit_tags>Tags: A</p>", "A"));
        }

        [Test]
        public void CommentedLineShouldNotMatchTag() {
            ClassicAssert.IsTrue(MatchesFilter(@"<!--<p id='fit_tags'>Tags: A</p>-->", "A"));
        }

        [Test]
        public void ShouldMatchAnySubsetOfTags() {
            ClassicAssert.IsTrue(MatchesFilter(@"<p id=""fit_tags"">Tags: A,B</p>", "A"));
        }

        [Test]
        public void ShouldNotMatchLessThanFilterRequires() {
            ClassicAssert.IsFalse(MatchesFilter(@"<p id=""fit_tags"">Tags: A</p>", "A,B"));
        }

        [Test]
        public void ShouldMatchTagIfContentContainsTwoDivElements() {
            ClassicAssert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: B</div><div class=""sidebar""> testing </div>", "B"));
        }

        [Test]
        public void ShouldHandleFormattedHtml() {
            string html = @"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">" + Environment.NewLine +
                          @"      Tags: B" + Environment.NewLine +
                          @"</div>";

            ClassicAssert.IsTrue(MatchesFilter(html, "B"));
        }

        [Test]
        public void ShouldAllowAnyTagLabel() {
            const string html = @"<div id=""fit_tags"">Anything: A</div>";
            ClassicAssert.IsTrue(MatchesFilter(html, "A"));
            ClassicAssert.IsFalse(MatchesFilter(html, "B"));
        }

        [Test]
        public void ShouldFindTagsIrrespectiveOfAttributeOrdering() {
            const string html = @"<div style=""color:gray;font-weight:bold;margin-bottom:1em;"" id=""fit_tags"">Tags: A</div>";
            ClassicAssert.IsTrue(MatchesFilter(html, "A"));
        }

        [Test]
        public void ShouldMatchWithAnyNumberOfQuotes() {
            ClassicAssert.IsTrue(MatchesFilter(@"<p id=""""fit_tags"""">Tags: A</p>", "A"));
        }

        [Test]
        public void MatchWildcardAsterisk() {
            ClassicAssert.IsTrue(MatchesFilter("<div id=fit_tags>Tags: bike</div>", "bi*"));
        }

        [Test]
        public void MatchWildcardQuestionMark() {
            ClassicAssert.IsTrue(MatchesFilter("<div id=fit_tags>Tags: car</div>", "c?r"));
        }

        private static bool MatchesFilter(string content, string tagList) {
            var page = new StubStoryTestPage(content);
            var testparser = new TagFilter(tagList);

            return (testparser.Matches(page));
        }

        private class StubStoryTestPage : StoryTestPage {
            readonly string content;

            public StubStoryTestPage(string content) {
                this.content = content;
            }

            public StoryPageName Name { get { return new StoryFileName("unused"); } }
            public string Content{ get { return content; } }
            public void WriteTest(PageResult result) {}
            public void WriteNonTest() {}
            public StoryTestSource TestContent { get { return StoryTestSource.FromString(content); } }
            public string OutputFile { get { return string.Empty;} }
            public bool HasTestContent { get { return true; } }

        }
    }
}
