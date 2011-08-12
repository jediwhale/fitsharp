// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using NUnit.Framework;
using fitSharp.Fit.Runner;

namespace fitSharp.Test.NUnit.Fit {

    [TestFixture]
    public class TagFilterTest {

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorShouldNotAllowEmptyFilter() {
            new TagFilter("");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorShouldNotAllowNullFilter() {
            new TagFilter(null);
        }

        [Test]
        public void MatchItemInTags() {
            Assert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", "A,B"));
        }
        
        [Test]
        public void MatchItemInTagsWithSpaces() {
            Assert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", " A, B  "));            
        }
        [Test]
        public void MatchTagWithOnlyOneTagItem() {
            Assert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: B</div>", "B"));            
        }
        [Test]
        public void ShouldNotMatchItemNotInFilter() {
            Assert.IsFalse(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: B</div>", "B-L"));
        }

        [Test]
        public void CaseInsensitiveIdMatch() {
            Assert.IsTrue(MatchesFilter(@"<div id=""FiT_TagS"">Tags: A</div>", "A"));
        }

        [Test]
        public void CaseInsensitiveTagMatch() {
            Assert.IsTrue(MatchesFilter(@"<p id=""fit_tags"">Tags: A</p>", "a"));
        }

        [Test]
        public void ShouldNotMatchIfNoIdAttribute() {
            Assert.IsFalse(MatchesFilter(@"<div ""fit_tags"">Tags: A</div>", "A"));
        }

        [Test]
        public void ShouldNotMatchContentWithoutTags() {
          Assert.IsFalse(MatchesFilter(@"<p>Content</p>", "A"));
        }

        [Test]
        public void ShouldNotMatchItemUnlessIdAttributeIs_fit_tags() {
            Assert.IsFalse(MatchesFilter(@"<div id=""fit_"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", "A, B"));
            Assert.IsFalse(MatchesFilter(@"<div id=""fit_Tags_testing"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: A, B</div>", "A, B"));
        }

        [Test]
        public void ShouldMatchWithoutLeadingTagsLabel() {
            Assert.IsTrue(MatchesFilter(@"<div id=""fit_tags"">A,B</div>", "A, B"));
        }

        [Test]
        public void ShouldMatchWithParagraphElement() {
            Assert.IsTrue(MatchesFilter(@"<p id=""fit_tags"">Tags: A,B</p>", "A, B"));
        }

        [Test]
        public void ShouldNotMatchTagSpecificationWithinComment() {
            Assert.IsFalse(MatchesFilter(@"<!-- p id=""fit_tags"">Tags: A,B</p -->", "A, B"));
        }

        [Test]
        public void ShouldNotMatchPartsOfTagNames() {
            Assert.IsFalse(MatchesFilter(@"<p id=""fit_tags"">Tags: B-L</p>", "B"));
        }

        [Test]
        public void AllowsFitTagsWithSingleQuotes() {
            Assert.IsTrue(MatchesFilter(@"<p id='fit_tags'>Tags: A</p>", "A"));
        }

        [Test]
        public void AllowsFitTagsWithoutQuotes() {
            Assert.IsTrue(MatchesFilter(@"<p id=fit_tags>Tags: A</p>", "A"));
        }

        [Test]
        public void CommentedLineShouldNotMatchTag() {
            Assert.IsTrue(MatchesFilter(@"<!--<p id='fit_tags'>Tags: A</p>-->", "A"));
        }

        [Test]
        public void ShouldMatchAnySubsetOfTags() {
            Assert.IsTrue(MatchesFilter(@"<p id=""fit_tags"">Tags: A,B</p>", "A"));
        }

        [Test]
        public void ShouldNotMatchLessThanFilterRequires() {
            Assert.IsFalse(MatchesFilter(@"<p id=""fit_tags"">Tags: A</p>", "A,B"));
        }

        [Test]
        public void ShouldMatchTagIfContentContainsTwoDivElements() {
            Assert.IsTrue(MatchesFilter(@"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">Tags: B</div><div class=""sidebar""> testing </div>", "B"));
        }

        [Test]
        public void ShouldHandleFormattedHtml() {
            string html = @"<div id=""fit_tags"" style=""color:gray; font-weight:bold; margin-bottom:1em;"">" + Environment.NewLine +
                          @"      Tags: B" + Environment.NewLine +
                          @"</div>";

            Assert.IsTrue(MatchesFilter(html, "B"));
        }

        [Test]
        public void ShouldAllowAnyTagLabel() {
            const string html = @"<div id=""fit_tags"">Anything: A</div>";
            Assert.IsTrue(MatchesFilter(html, "A"));
            Assert.IsFalse(MatchesFilter(html, "B"));
        }

        [Test]
        public void ShouldFindTagsIrrespectiveOfAttributeOrdering() {
            const string html = @"<div style=""color:gray;font-weight:bold;margin-bottom:1em;"" id=""fit_tags"">Tags: A</div>";
            Assert.IsTrue(MatchesFilter(html, "A"));
        }

        [Test]
        public void ShouldMatchWithAnyNumberOfQuotes() {
            Assert.IsTrue(MatchesFilter(@"<p id=""""fit_tags"""">Tags: A</p>", "A"));
        }

        [Test]
        public void MatchWildcardAsterisk() {
            Assert.IsTrue(MatchesFilter("<div id=fit_tags>Tags: bike</div>", "bi*"));
        }

        [Test]
        public void MatchWildcardQuestionMark() {
            Assert.IsTrue(MatchesFilter("<div id=fit_tags>Tags: car</div>", "c?r"));
        }

        private static bool MatchesFilter(string content, string tagList) {
            var page = new StubStoryTestPage(content);
            var testparser = new TagFilter(tagList);

            return (testparser.Matches(page));
        }

        private class StubStoryTestPage : StoryTestPage
        {
            readonly string content;

            public StubStoryTestPage(string content)
            {
                this.content = content;
            }

            public StoryPageName Name
            {
                get { return new StoryFileName("unused"); }
            }

            public string Content
            {
                get { return content; }
            }

            public void ExecuteStoryPage(Action<StoryPageName, fitSharp.Fit.Model.StoryTestString, Action<string, fitSharp.Fit.Model.TestCounts>, Action> executor, fitSharp.Fit.Service.ResultWriter resultWriter, Action<fitSharp.Fit.Model.TestCounts> handler)
            {
                // Nothing to do here
            }
        }
    }
}
