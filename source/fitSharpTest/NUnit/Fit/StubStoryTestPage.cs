using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Fit.Runner;

namespace fitSharp.Test.NUnit.Fit
{
    public class StubStoryTestPage : StoryTestPage
    {
        string content;

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

        public void ExecuteStoryPage(Action<StoryPageName, fitSharp.Fit.Model.StoryTestString, Action<fitSharp.Fit.Model.StoryTestString, fitSharp.Fit.Model.TestCounts>, Action> executor, fitSharp.Fit.Service.ResultWriter resultWriter, Action<fitSharp.Fit.Model.TestCounts> handler)
        {
            // Nothing to do here
        }
    }
}
