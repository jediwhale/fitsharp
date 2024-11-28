// Copyright � 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Exception;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class FindMemberPatternTest {

        [Test] public void MethodWithMemberPatternAttributeIsInvoked() {
            InvokeMemberPattern("do math with sally", "I did math with sally");
        }

        static void InvokeMemberPattern(string memberPattern, string expectedResult) {
            var processor = Builder.CellProcessor();
            var findMember = new FindMemberPattern { Processor = processor };
            var instance = new TestClass();
            var query = new MemberQuery(new MemberSpecification(new MemberName(memberPattern), 0));
            var member = findMember.FindMember(new TypedValue(instance), query);
            ClassicAssert.IsTrue(member.HasValueAs<RuntimeMember>());
            member.GetValueAs<RuntimeMember>().Invoke(new object[] {});
            ClassicAssert.AreEqual(expectedResult, instance.Field);
        }

        [Test] public void MethodWithMemberPatternAttributeParsesParameters() {
            InvokeMemberPattern("do math 3 times", "I did math math math");
        }

        [Test] public void MethodWithMisMatchedParameters() {
            try {
                InvokeMemberPattern("do math extra parameter", "I did math");
                ClassicAssert.Fail();
            }
            catch (InvalidMethodException e) {
                ClassicAssert.AreEqual("Member pattern for fitSharp.Test.NUnit.Fit.FindMemberPatternTest+TestClass:MisMatchedParameters(1) has 2 parameters.", e.Message);
            }
        }
 
        class TestClass {
            public string Field;

            [MemberPattern("do (.*) with (.*)")]
            public void DoStuff(string what, string withWhom) {
                Field = "I did " + what + " with " + withWhom;
            }

            [MemberPattern("do (.*) (.*) times")]
            public void DoTimes(string what, int times) {
                Field = "I did";
                for (var i = 0; i < times; i++) Field = Field + " " + what;
            }

            [MemberPattern("do (.*) extra (.*)")]
            public void MisMatchedParameters(string what) {
                Field = "I did " + what;
            }
        }
    }
}
