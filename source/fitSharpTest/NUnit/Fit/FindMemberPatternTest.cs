// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class FindMemberPatternTest {

        [Test] public void MethodWithMemberPatternAttributeIsInvoked() {
            InvokeMemberPattern("do math with sally", "I did math with sally");
        }

        static void InvokeMemberPattern(string memberPattern, string expectedResult) {
            var processor = Builder.CellProcessor();
            var findMember = new FindMemberPattern { Processor = processor };
            var instance = new SampleClass();
            var query = new MemberQuery(new MemberSpecification(new MemberName(memberPattern), 0));
            var member = findMember.FindMember(new TypedValue(instance), query);
            Assert.IsTrue(member.HasValueAs<RuntimeMember>());
            member.GetValueAs<RuntimeMember>().Invoke(new object[] {});
            Assert.AreEqual(expectedResult, instance.Field);
        }

        [Test] public void MethodWithMemberPatternAttributeParsesParameters() {
            InvokeMemberPattern("do math 3 times", "I did math math math");
        } 
    }
}
