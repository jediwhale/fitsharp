// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using fitSharp.Test.Double;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class MemberQueryTest {
        SampleClass instance;

        [SetUp] public void SetUp() {
            instance = new SampleClass();
        }

        [Test] public void VoidMethodIsInvoked() {
            var result = Invoke(GetMethod("countmethod", 0));
            ClassicAssert.AreEqual(null, result.Value);
            ClassicAssert.AreEqual(typeof(void), result.Type);
            ClassicAssert.AreEqual(1, instance.Count);
        }

        Maybe<RuntimeMember> GetMethod(string memberName, int count) {
            return MemberQuery.FindInstance(MemberQuery.FindMember, TypedValue.Of(instance),
                    new MemberSpecification(new MemberName(memberName), count));
        }

        TypedValue Invoke(Maybe<RuntimeMember> method) {
            return Invoke(method, Array.Empty<object>());
        }
        
        TypedValue Invoke(Maybe<RuntimeMember> method, object[] parameters) {
            ClassicAssert.True(method.IsPresent);
            return method.Select(m => m.Invoke(parameters)).OrElse(TypedValue.MakeInvalid(new NullReferenceException("method not present")));
        }

        [Test] public void MethodIsInvokedViaProcessor() {
            var result = Invoke(GetMethodFromProcessor(instance, "countmethod", 0));
            ClassicAssert.AreEqual(null, result.Value);
            ClassicAssert.AreEqual(typeof(void), result.Type);
            ClassicAssert.AreEqual(1, instance.Count);
        }

        static Maybe<RuntimeMember> GetMethodFromProcessor(object targetInstance, string memberName, int parameterCount) {
            var processor = Builder.CellProcessor();
            return MemberQuery.FindInstance(processor.FindMember, TypedValue.Of(targetInstance),
                    new MemberSpecification(new MemberName(memberName), parameterCount));
        }

        [Test] public void MethodWithReturnIsInvoked() {
            var result = Invoke(GetMethod("methodnoparms", 0));
            ClassicAssert.AreEqual("samplereturn", result.Value.ToString());
            ClassicAssert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void MethodWithUnderscoresIsInvoked() {
            var result = Invoke(GetMethod("methodwithunderscores", 0));
            ClassicAssert.AreEqual("samplereturn", result.Value.ToString());
        }

        [Test] public void MethodWithParmsIsInvoked() {
            var result = Invoke(GetMethod("methodwithparms", 1), new object[] {"input"});
            ClassicAssert.AreEqual("sampleinput", result.Value.ToString());
        }

        [Test] public void GenericMethodWithParmsIsInvoked() {
            var member = new MemberName("genericmethodofsystemint32", "genericmethod", new[] { typeof(int)});
            var method = MemberQuery.FindInstance(MemberQuery.FindMember, TypedValue.Of(instance),
                    new MemberSpecification(member, 1));
            var result = Invoke(method, new object[] {123});
            ClassicAssert.AreEqual("sample123", result.Value.ToString());
        }

        [Test] public void StaticMethodWithParmsIsInvoked() {
            foreach (var method in new RuntimeType(instance.GetType()).FindStatic(MemberName.ParseMethod, new [] {typeof(string)}).Value) {
                var result = method.Invoke(new object[] {"input"});
                ClassicAssert.AreEqual(typeof (SampleClass), result.Type);
                return;
            }
            ClassicAssert.Fail("no method");
        }

        [Test] public void ConstructorIsInvoked() {
            var method = new RuntimeType(instance.GetType()).GetConstructor(0);
            ClassicAssert.IsNotNull(method);
            var result = method.Invoke(new object[] {});
            ClassicAssert.AreEqual(typeof(SampleClass), result.Type);
        }

        [Test] public void PropertySetAndGetIsInvoked() {
            var result = Invoke(GetMethod("property", 1), new object[] {"stuff"});
            ClassicAssert.AreEqual(null, result.Value);
            ClassicAssert.AreEqual(typeof(void), result.Type);

            result = Invoke(GetMethod("property", 0));
            ClassicAssert.AreEqual("stuff", result.Value.ToString());
            ClassicAssert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void IndexerIsInvoked() {
            var result = Invoke(GetMethod("anything", 0));
            ClassicAssert.AreEqual("indexanything", result.Value);
            ClassicAssert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void FieldIsInvokedWithGetAndSet() {
            var result = Invoke(GetMethod("setfield", 1), new object[] {"stuff"});
            ClassicAssert.AreEqual(null, result.Value);
            ClassicAssert.AreEqual(typeof(void), result.Type);

            result = Invoke(GetMethod("getfield", 0));
            ClassicAssert.AreEqual("stuff", result.Value.ToString());
            ClassicAssert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void DuplicateIsInvoked() {
            Invoke(GetMethod("duplicate", 1), new object[] {"stuff"});

            var result = Invoke(GetMethod("duplicate", 0));
            ClassicAssert.AreEqual("stuff", result.ToString());
        }

        [Test] public void QueryableMemberIsInvoked() {
            var queryable = new QueryableClass();
            var method = GetMethodFromProcessor(queryable, "dynamic", 1);
            var result = Invoke(method, new object[] {"stuff"});
            ClassicAssert.AreEqual("dynamicstuff", result.Value);
        }

        [Test]
        public void ExtensionMethodIsInvoked() {
            var name = new MemberName("increase(sampleextension)", "increase", Maybe<Type>.Of(typeof(SampleExtension)), Array.Empty<Type>());
            var method = MemberQuery.FindInstance(MemberQuery.FindMember, TypedValue.Of(instance), new MemberSpecification(name, 1));
            var result = Invoke(method, new object[] {2});
            ClassicAssert.AreEqual(2, result.Value);
        }

        class QueryableClass: MemberQueryable {
            public RuntimeMember Find(MemberSpecification specification) {
                ClassicAssert.IsTrue(specification.MatchesIdentifierName("dynamic"));
                return new QueryableMember("dynamic");
            }

            class QueryableMember: RuntimeMember {
                public QueryableMember(string memberName) {
                    Name = memberName;
                }

                public TypedValue Invoke(object[] parameters) {
                    return new TypedValue(Name + parameters[0]);
                }

                public bool MatchesParameterCount(int count) {
                    throw new NotImplementedException();
                }

                public Type GetParameterType(int index) {
                    throw new NotImplementedException();
                }

                public string GetParameterName(int index) {
                  throw new NotImplementedException();
                }

                public Type ReturnType => throw new NotImplementedException();

                public string Name { get; }
            }
        }

        [Test] public void MethodwithMisMatchedParameterNamesIsNotFound() {
            ClassicAssert.False(MemberQuery.FindInstance(MemberQuery.FindMember, TypedValue.Of(instance),
                new MemberSpecification("methodwithparms", 1).WithParameterNames(new [] {"garbage"})).IsPresent);
        }

        [Test] public void MethodwithMatchedParameterNamesIsFound() {
            ClassicAssert.True(MemberQuery.FindInstance(MemberQuery.FindMember, TypedValue.Of(instance),
                new MemberSpecification("methodwithparms", 1).WithParameterNames(new [] {"input"})).IsPresent);
        }
    }
}
