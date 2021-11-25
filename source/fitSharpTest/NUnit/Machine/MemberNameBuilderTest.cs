// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture]
    public class MemberNameBuilderTest {

        [Test] public void ParsesGenericType() {
            var member = Parse("generic of System.String");
            Assert.AreEqual("genericofsystemstring", member.Name);
            Assert.True(member.Matches(typeof(MemberNameBuilder).GetMethod("Generic")));
        }
        
        [Test] public void ParsesGenericTypeWithUnderscores() {
            var member = Parse("generic_of_System.String");
            Assert.True(member.Matches(typeof(MemberNameBuilder).GetMethod("Generic")));
        }
        
        [Test]
        public void ParsesExtensionMethod() {
            var member = Parse("extension in member name builder extension");
            Assert.True(member.Matches(typeof(MemberNameBuilderExtension).GetMethod("Extension")));
        }

        [Test]
        public void ParsesExtensionMethodWithUnderscores() {
            var member = Parse("extension_in_MemberNameBuilderExtension");
            Assert.True(member.Matches(typeof(MemberNameBuilderExtension).GetMethod("Extension")));
        }

        static MemberName Parse(string name) {
            var application = new ApplicationUnderTest();
            application.AddAssembly(typeof(MemberNameBuilderTest).Assembly.Location);
            application.AddNamespace("fitSharp.Test.NUnit.Machine");
            return new fitSharp.Machine.Engine.MemberNameBuilder(application).MakeMemberName(name);
        }
    }
    
    public class MemberNameBuilder {
        public bool Generic<T>() { return true; }
    }

    public static class MemberNameBuilderExtension {
        public static void Extension(this MemberNameBuilder nameBuilder) {}
    }
}
