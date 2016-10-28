using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Runner;

namespace fitIf {
    public class StoryTestSuite {
        private readonly Func<string, bool> contentFilter;

        public StoryTestSuite(Func<string, bool> contentFilter ) {
            this.contentFilter = contentFilter;
        }

        public IEnumerable<string> TestNames(Folder folder) {
            return folder.Pages.Items.Where(IsStoryTestPage).Select(page => page.Name);
        }

        bool IsStoryTestPage(Page page) {
            var storyFileName = new StoryFileName(page.Name);
            if (storyFileName.IsSetUp || storyFileName.IsTearDown) return false;
            return contentFilter(page.Content());
        }

    }
}
