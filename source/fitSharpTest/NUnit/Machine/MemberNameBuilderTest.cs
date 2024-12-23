// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture]
    public class MemberNameBuilderTest {

        [Test] public void ParsesGenericTypeWithBlanks() {
            // old behaviour, preserve for compatibility
            var member = Parse("generic of some class");
            ClassicAssert.AreEqual("genericofsomeclass", member.Name);
            ClassicAssert.True(member.Matches(typeof(BuilderSample).GetMethod("Generic")));
        }
        
        [Test] public void ParsesGenericType() {
            var member = Parse("generic.of.some class");
            var methodInfo = typeof(BuilderSample).GetMethod("Generic");
            ClassicAssert.True(member.Matches(methodInfo));
            var genericMember = member.MakeMember(methodInfo, new BuilderSample());
            ClassicAssert.AreEqual("SomeClass", genericMember.Invoke(new object [] {}).Value);
        }

        [Test] public void ParsesMultipleGenericTypes() {
            var member = Parse("multi generic.of.some class.of.some other class");
            var methodInfo = typeof(BuilderSample).GetMethod("MultiGeneric");
            ClassicAssert.True(member.Matches(methodInfo));
            var genericMember = member.MakeMember(methodInfo, new BuilderSample());
            ClassicAssert.AreEqual("SomeClassSomeOtherClass", genericMember.Invoke(new object [] {}).Value);
        }
        
        [Test]
        public void ParsesExtensionMethod() {
            var member = Parse("extension.in.some extensions");
            ClassicAssert.True(member.Matches(typeof(SomeExtensions).GetMethod("Extension")));
        }

        [Test]
        public void ParsesMethodWithEmbeddedOf() {
            var member = Parse("not of generic");
            ClassicAssert.True(member.Matches(typeof(BuilderSample).GetMethod("NotOfGeneric")));
        }

        [Test]
        public void ParsesGenericExtensionMethod() {
            ClassicAssert.True(Parse("generic.of.some class.in.some extensions")
                .Matches(typeof(SomeExtensions).GetMethod("Generic")));
        }

        static MemberName Parse(string name) {
            var application = new ApplicationUnderTest();
            application.AddAssembly(typeof(MemberNameBuilderTest).Assembly.Location);
            application.AddNamespace(typeof(MemberNameBuilderTest).Namespace);
            return new MemberNameBuilder(application).MakeMemberName(name);
        }
    }
    
    public class BuilderSample {
        public string Generic<T>() { return typeof(T).Name; }
        public string MultiGeneric<T,U>() { return typeof(T).Name + typeof(U).Name; }
        public int NotOfGeneric() { return 0; }
    }
    
    public class SomeClass {}
    public class SomeOtherClass {}

    public static class SomeExtensions {
        public static void Extension(this BuilderSample nameBuilder) {}
        public static void Generic<T>(this BuilderSample nameBuilder) {}
    }
}
