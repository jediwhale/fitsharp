// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class BindingFactoryTest {

        [Test] public void BindsNoActionForEmptyString() {
            Assert.IsTrue(Bind(string.Empty) is NoBinding);
        }

        static BindingOperation Bind(string input) {
            return new BindingFactory(Builder.CellProcessor(), null, new TestTarget()).Make(new CellTreeLeaf(input));
        }

        [Test] public void BindsCheckForQuestionSuffix() {
            Assert.IsTrue(Bind("stuff?") is CheckBinding);
        }

        [Test] public void BindsCheckForExclamationSuffix() {
            Assert.IsTrue(Bind("stuff!") is CheckBinding);
        }

        [Test] public void BindsCheckForParenthesesSuffix() {
            Assert.IsTrue(Bind("stuff()") is CheckBinding);
        }

        [Test] public void BindsInputForPlainString() {
            Assert.IsTrue(Bind("stuff") is InputBinding);
        }

        [Test] public void BindsCreateForNewPrefix() {
            Assert.IsTrue(Bind("new stuff") is CreateBinding);
        }

        [Test] public void BindsCheckForNewPrefixAndQuestionSuffix() {
            Assert.IsTrue(Bind("new stuff?") is CheckBinding);
        }

        [Test] public void BindsInputForNewPrefixOnMemberName() {
            Assert.IsTrue(Bind("new member") is InputBinding);
        }

        class TestTarget: TargetObjectProvider {
            public string NewMember;
            public object GetTargetObject() { return this; }
        }
    }
}
