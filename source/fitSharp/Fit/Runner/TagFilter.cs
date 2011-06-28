// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace fitSharp.Fit.Runner {

    public class TagFilter : StoryTestPageFilter {

        private class TagPredicate {
            readonly Regex expression;

            public TagPredicate(string filter) {
                expression = new Regex(ConvertWildcardToRegex(filter), RegexOptions.IgnoreCase);
            }

            public bool Matches(IEnumerable<string> tags) {
                return tags.Any(tag => expression.IsMatch(tag));
            }

            private static string ConvertWildcardToRegex(string filter) {
                string pattern = Regex.Escape(filter);
                pattern = pattern.Replace("\\*", ".*");
                pattern = pattern.Replace("\\?", ".");
                pattern = "^" + pattern + "$";
                return pattern;
            }
        }

        private const string TagPattern = @"<[^!][a-z0-9]*\s+.*id\s*=\s*('+fit_tags'+|""+fit_tags""+|fit_tags)[^>]*>(?<Tags>[^<]+)</";

        private readonly IEnumerable<TagPredicate> tagPredicates;
        private readonly Regex tagExpression;

        public TagFilter(string tagFilters) {
            if (string.IsNullOrEmpty(tagFilters))
                throw new ArgumentException("Tag list is either empty or null");

            tagPredicates = ParseTagPredicates(tagFilters);
            tagExpression = new Regex(TagPattern, RegexOptions.IgnoreCase);
        }

        public bool Matches(StoryTestPage page) {
            string pageTags = ExtractPageTags(page);
            return MatchesFilter(pageTags);
        }

        private string ExtractPageTags(StoryTestPage page) {
            Match matchingContent = tagExpression.Match(page.Content);
            string rawTagsValue = matchingContent.Groups["Tags"].Value;

            return TrimTagLabel(rawTagsValue);
        }
                
        private static string TrimTagLabel(string rawTagsValue) {
            string tagSpecification = rawTagsValue;

            int colonIndex = tagSpecification.IndexOf(':');
            if (colonIndex > -1)
                tagSpecification = tagSpecification.Substring(colonIndex + 1);

            return tagSpecification;
        }

        private bool MatchesFilter(string pageTagList) {
            IEnumerable<string> pageTags = ParseTagList(pageTagList);

            return tagPredicates.All(predicate => predicate.Matches(pageTags));
        }

        private static IEnumerable<string> ParseTagList(string tagList) {
            string[] tags = tagList.Split(',');

            for (int i = 0; i < tags.Length; ++i) {
                tags[i] = tags[i].Trim();
            }

            return tags;
        }

        private static IEnumerable<TagPredicate> ParseTagPredicates(string tagFilters) {
            IEnumerable<string> filters = ParseTagList(tagFilters);
            return filters.Select(filter => new TagPredicate(filter));
        }
    }
}