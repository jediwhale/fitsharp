// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class GracefulNameTest {

        [Test] public void TestGracefulNameConverterOnTypeName() {
            AssertConverts("_badcompany_", "?&_)*( bad&^%$*()(*&)compAny~`+=-_,");
            AssertConverts("badcompany", "Bad.Company");
            AssertConverts("badcompany", "BadCompany");
            AssertConverts("badcompany", "Bad Company");
            AssertConverts("badcompany", "bad company");
            AssertConverts("badcompany", "Bad-Company");
            AssertConverts("badcompany", "Bad Company.");
            AssertConverts("badcompany", "(Bad Company)");
            AssertConverts("bad123company", "bad 123 company");
            AssertConverts("bad123company", "bad 123company");
            AssertConverts("bad123company", "   bad  \t123  company   ");
        }

        static void AssertConverts(string expected, string input) {
            Assert.AreEqual(expected, new GracefulName(input).IdentifierName.ToString());
        }

        [Test] public void TestConvertMemberName() {
            AssertConverts("somemembername", "Some Member Name");
            AssertConverts("somemembername", "Some Member Name?");
            AssertConverts("somemembername", "Some Member Name!");
            AssertConverts("somemembername", "Some Member Name()");
            AssertConverts("member1name", "Member 1 Name.");
        }

        [Test] public void LeavesQuotedNameAsIs() {
            AssertConverts("some_name", "\"some_name\"");
        }
    }
}
