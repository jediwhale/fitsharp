// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class BindingFactoryTest {

        [Test] public void BindsNoActionForEmptyString() {
            ClassicAssert.IsTrue(Bind(string.Empty) is NoBinding);
        }

        [Test] public void BindsCheckForQuestionSuffix() {
            ClassicAssert.IsTrue(Bind("stuff?") is CheckBinding);
        }

        [Test] public void BindsCheckForExclamationSuffix() {
            ClassicAssert.IsTrue(Bind("stuff!") is CheckBinding);
        }

        [Test] public void BindsCheckForParenthesesSuffix() {
            ClassicAssert.IsTrue(Bind("stuff()") is CheckBinding);
        }

        [Test] public void BindsInputForPlainString() {
            ClassicAssert.IsTrue(Bind("stuff") is InputBinding);
        }

        [Test] public void BindsCreateForNewPrefix() {
            ClassicAssert.IsTrue(Bind("new stuff") is CreateBinding);
        }

        [Test] public void BindsInputForNewPrefixWithoutSpace() {
            ClassicAssert.IsTrue(Bind("newstuff") is InputBinding);
        }

        [Test] public void BindsCheckForNewPrefixAndQuestionSuffix() {
            ClassicAssert.IsTrue(Bind("new stuff?") is CheckBinding);
        }

        [Test] public void BindsInputForNewPrefixOnMemberName() {
            ClassicAssert.IsTrue(Bind("new member") is InputBinding);
        }

        static BindingOperation Bind(string input) {
            return new BindingFactory(Builder.CellProcessor(), null, new TestTarget()).Make(new CellTreeLeaf(input));
        }

        class TestTarget: TargetObjectProvider {
            public string NewMember = null;
            public object GetTargetObject() { return this; }
        }
    }
}
