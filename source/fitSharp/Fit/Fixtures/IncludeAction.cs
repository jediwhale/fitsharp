// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.IO;
using fitSharp.Machine.Application;

namespace fitSharp.Fit.Fixtures {
    public class IncludeAction {
        public enum PageBase {
            FromWorking,
            FromCurrent,
            FromSuite
        }

        public IncludeAction(CellProcessor processor) {
            this.processor = processor;
        }

        public string Result { get; private set; }

        public void Page(string pageName) {
            Page(PageBase.FromWorking, pageName);
        }

        public void Page(PageBase pageBase, string pageName) {
            var pageSource = processor.Get<Context>().PageSource;
            String(pageBaseActions[pageBase](processor, pageSource.MakePath(pageName)));
        }

        public void Text<T>(T storyTestSource) {
            String(storyTestSource.ToString());
        }

        public void String(string storyTestText) {
            var writer = new StoryTestStringWriter(processor);
            var storyTest = new StoryTest(processor, writer).WithInput(storyTestText);
            if (storyTest.IsExecutable) {
                storyTest.Execute();
                Result = writer.Tables;
            }
            else {
                Result = storyTestText;
            }
        }

        static string WorkingPageBase(CellProcessor processor, Path pageName) {
            return processor.Get<Context>().PageSource.GetPageContent(pageName);
        }

        static string CurrentPageBase(CellProcessor processor, Path pageName) {
            var pageSource = processor.Get<Context>().PageSource;
            return pageSource.GetPageContent(processor.Get<Context>().TestPagePath.WithSubPath(pageName));
        }

        static string SuitePageBase(CellProcessor processor, Path pageName) {
            var pageSource = processor.Get<Context>().PageSource;
            return pageSource.GetPageContent(processor.Get<Context>().SuitePath.WithSubPath(pageName));
        }

        readonly CellProcessor processor;

        readonly Dictionary<PageBase, Func<CellProcessor, Path, string>> pageBaseActions
            = new Dictionary<PageBase, Func<CellProcessor, Path, string>> {
            {PageBase.FromWorking, WorkingPageBase},
            {PageBase.FromCurrent, CurrentPageBase},
            {PageBase.FromSuite, SuitePageBase}
        };
    }
}
