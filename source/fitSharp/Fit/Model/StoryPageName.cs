// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Fit.Model {
    public interface StoryPageName {
        string Name { get; }
        bool IsSuitePage { get; }
        bool IsSuiteSetUp { get; }
        bool IsSuiteTearDown { get; }
        bool IsExcelSpreadsheet { get;}
    }
}
