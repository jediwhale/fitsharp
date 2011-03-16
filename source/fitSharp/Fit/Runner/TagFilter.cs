using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace fitSharp.Fit.Runner {

    public class TagFilter : StoryTestPageFilter {

        private class TagPredicate {
            Regex expression;

            public TagPredicate(string filter) {
                expression = new Regex(ConvertWildcardToRegex(filter), RegexOptions.IgnoreCase);
            }

            public bool Matches(IEnumerable<string> tags) {
                foreach (string tag in tags) {
                    if (expression.IsMatch(tag))
                        return true;
                }

                return false;
            }

            private string ConvertWildcardToRegex(string filter) {
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
                
        private string TrimTagLabel(string rawTagsValue) {
            string tagSpecification = rawTagsValue;

            int colonIndex = tagSpecification.IndexOf(':');
            if (colonIndex > -1)
                tagSpecification = tagSpecification.Substring(colonIndex + 1);

            return tagSpecification;
        }

        private bool MatchesFilter(string pageTagList) {
            IEnumerable<string> pageTags = ParseTagList(pageTagList);

            foreach (TagPredicate predicate in tagPredicates) {
                if (!predicate.Matches(pageTags))
                    return false;
            }

            return true;
        }

        private IEnumerable<string> ParseTagList(string tagList) {
            string[] tags = tagList.Split(',');

            for (int i = 0; i < tags.Length; ++i) {
                tags[i] = tags[i].Trim();
            }

            return tags;
        }

        private IEnumerable<TagPredicate> ParseTagPredicates(string tagFilters) {
            IEnumerable<string> filters = ParseTagList(tagFilters);
            return filters.Select(filter => new TagPredicate(filter));
        }
    }
}