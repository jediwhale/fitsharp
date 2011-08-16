// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.IO;

namespace fitSharp.Fit.Model {
    public class PageResult {
        public string Content { get; private set; }
        public string Title { get; private set; }
        public TestCounts TestCounts { get; private set; }
        public ElapsedTime ElapsedTime { get; private set; }

        public PageResult(string title, string content, TestCounts testCounts, ElapsedTime elapsedTime) {
            Title = title;
            Content = content;
            TestCounts = testCounts;
            ElapsedTime = elapsedTime;
        }
        
        public PageResult(string title, string content, TestCounts testCounts):
            this(title, content, testCounts, new ElapsedTime()) {}
    }
}