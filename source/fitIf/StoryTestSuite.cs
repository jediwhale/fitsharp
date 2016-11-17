using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Application;
using fitSharp.Fit.Runner;

namespace fitIf {
    public class StoryTestSuite {

        public StoryTestSuite(FileExclusions exclusions, Func<string, bool> contentFilter ) {
            this.exclusions = exclusions;
            this.contentFilter = contentFilter;
        }

        public IEnumerable<Folder> SubFolders(Folder folder) {
            return folder.Folders.Items.Where(IsStoryTestFolder);
        }

        public IEnumerable<string> TestNames(Folder folder) {
            return folder.Pages.Items.Where(IsStoryTestPage).Select(page => page.Name);
        }

        bool IsStoryTestFolder(Folder folder) {
            return !exclusions.IsExcluded(folder.Name());
        }

        bool IsStoryTestPage(Page page) {
            if (exclusions.IsExcluded(page.Name)) return false;
            var storyFileName = new StoryFileName(page.Name);
            if (storyFileName.IsSetUp || storyFileName.IsTearDown) return false;
            return contentFilter(page.Content());
        }

        readonly FileExclusions exclusions;
        readonly Func<string, bool> contentFilter;
    }
}
