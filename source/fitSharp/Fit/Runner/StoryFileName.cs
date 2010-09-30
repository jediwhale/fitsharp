// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Runner {
    public interface StoryPageName {
        string Name { get; }
        bool IsSuitePage { get; }
    }

    public class StoryFileName: StoryPageName {
        private const string outputExtension = ".html";

        public StoryFileName(string theName) {
            myName = theName;
        }

        public string Name { get { return myName; } }
        public string OutputFileName { get { return Path.GetFileNameWithoutExtension(myName) + outputExtension; } }
        public string CopyFileName { get { return Path.GetFileName(myName); } }

        public bool IsSuitePage { get { return IsSuiteSetUp || IsSuiteTearDown; } }

        public bool IsSetUp {
            get {
                string name = Path.GetFileName(myName);
                return ourSetupIdentifier1.Equals(name) || ourSetupIdentifier2.Equals(name);
            }
        }

        public bool IsSuiteSetUp {
            get {
                return ourSuiteSetupIdentifier.Equals(OutputFileName);
            }
        }

        public bool IsSuiteTearDown {
            get {
                return ourSuiteTearDownIdentifier.Equals(OutputFileName);
            }
        }

        public bool IsTearDown {
            get {
                string name = Path.GetFileName(myName);
                return ourTeardownIdentifier1.Equals(name) || ourTeardownIdentifier2.Equals(name);
            }
        }

        private static readonly IdentifierName ourSetupIdentifier1 = new IdentifierName("setup.html");
        private static readonly IdentifierName ourSetupIdentifier2 = new IdentifierName("setup.htm");
        private static readonly IdentifierName ourTeardownIdentifier1 = new IdentifierName("teardown.html");
        private static readonly IdentifierName ourTeardownIdentifier2 = new IdentifierName("teardown.htm");
        private static readonly IdentifierName ourSuiteSetupIdentifier = new IdentifierName("suitesetup.html");
        private static readonly IdentifierName ourSuiteTearDownIdentifier = new IdentifierName("suiteteardown.html");
        private readonly string myName;
    }
}
