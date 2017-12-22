// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Runner {
    public interface StoryPageName {
        string Name { get; }
        bool IsSuitePage { get; }
        bool IsSuiteSetUp { get; }
        bool IsSuiteTearDown { get; }
    }

    public class StoryFileName: StoryPageName {

        public StoryFileName(string theName) {
            Name = theName;
        }

        public string Name { get; }

        public string OutputFileName => Path.GetFileNameWithoutExtension(Name) + outputExtension;
        public string CopyFileName => Path.GetFileName(Name);
        public bool IsSuitePage => IsSuiteSetUp || IsSuiteTearDown;
        public bool IsSuiteSetUp => ourSuiteSetupIdentifier.Equals(OutputFileName);
        public bool IsSuiteTearDown => ourSuiteTearDownIdentifier.Equals(OutputFileName);

        public bool IsSetUp {
            get {
                var name = Path.GetFileName(Name);
                return ourSetupIdentifier1.Equals(name) || ourSetupIdentifier2.Equals(name);
            }
        }

        public bool IsTearDown {
            get {
                var name = Path.GetFileName(Name);
                return ourTeardownIdentifier1.Equals(name) || ourTeardownIdentifier2.Equals(name);
            }
        }

        const string outputExtension = ".html";

        static readonly IdentifierName ourSetupIdentifier1 = new IdentifierName("setup.html");
        static readonly IdentifierName ourSetupIdentifier2 = new IdentifierName("setup.htm");
        static readonly IdentifierName ourTeardownIdentifier1 = new IdentifierName("teardown.html");
        static readonly IdentifierName ourTeardownIdentifier2 = new IdentifierName("teardown.htm");
        static readonly IdentifierName ourSuiteSetupIdentifier = new IdentifierName("suitesetup.html");
        static readonly IdentifierName ourSuiteTearDownIdentifier = new IdentifierName("suiteteardown.html");
    }
}
