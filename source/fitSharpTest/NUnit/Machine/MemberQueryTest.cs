// Copyright © 2021 Syterra Software Inc. All rights reserved.
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

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class MemberQueryTest {
        SampleClass instance;

        [SetUp] public void SetUp() {
            instance = new SampleClass();
        }

        [Test] public void VoidMethodIsInvoked() {
            RuntimeMember method = GetMethod("countmethod", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);
            Assert.AreEqual(1, instance.Count);
        }

        RuntimeMember GetMethod(string memberName, int count) {
            return MemberQuery.FindInstance(MemberQuery.FindMember, instance,
                    new MemberSpecification(new MemberName(memberName), count));
        }

        [Test] public void MethodIsInvokedViaProcessor() {
            RuntimeMember method = GetMethodFromProcessor(instance, "countmethod", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);
            Assert.AreEqual(1, instance.Count);
        }

        static RuntimeMember GetMethodFromProcessor(object targetInstance, string memberName, int parameterCount) {
            var processor = Builder.CellProcessor();
            return MemberQuery.FindInstance(processor.FindMember, targetInstance,
                    new MemberSpecification(new MemberName(memberName), parameterCount));
        }

        [Test] public void MethodWithReturnIsInvoked() {
            RuntimeMember method = GetMethod("methodnoparms", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("samplereturn", result.Value.ToString());
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void MethodWithUnderscoresIsInvoked() {
            RuntimeMember method = GetMethod("methodwithunderscores", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("samplereturn", result.Value.ToString());
        }

        [Test] public void MethodWithParmsIsInvoked() {
            RuntimeMember method = GetMethod("methodwithparms", 1);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"input"});
            Assert.AreEqual("sampleinput", result.Value.ToString());
        }

        [Test] public void GenericMethodWithParmsIsInvoked() {
            var member = new MemberName("genericmethodofsystemint32", "genericmethod", new[] { typeof(int)});
            var method = MemberQuery.FindInstance(MemberQuery.FindMember, instance,
                    new MemberSpecification(member, 1));
            Assert.IsNotNull(method);
            var result = method.Invoke(new object[] {123});
            Assert.AreEqual("sample123", result.Value.ToString());
        }

        [Test] public void StaticMethodWithParmsIsInvoked() {
            foreach (var method in new RuntimeType(instance.GetType()).FindStatic(MemberName.ParseMethod, new [] {typeof(string)}).Value) {
                TypedValue result = method.Invoke(new object[] {"input"});
                Assert.AreEqual(typeof (SampleClass), result.Type);
                return;
            }
            Assert.Fail("no method");
        }

        [Test] public void ConstructorIsInvoked() {
            RuntimeMember method = new RuntimeType(instance.GetType()).GetConstructor(0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual(typeof(SampleClass), result.Type);
        }

        [Test] public void PropertySetAndGetIsInvoked() {
            RuntimeMember method = GetMethod("property", 1);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"stuff"});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);

            method = GetMethod("property", 0);
            Assert.IsNotNull(method);
            result = method.Invoke(new object[] {});
            Assert.AreEqual("stuff", result.Value.ToString());
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void IndexerIsInvoked() {
            RuntimeMember method = GetMethod("anything", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("indexanything", result.Value);
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void FieldIsInvokedWithGetAndSet() {
            RuntimeMember method = GetMethod("setfield", 1);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"stuff"});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);

            method = GetMethod("getfield", 0);
            Assert.IsNotNull(method);
            result = method.Invoke(new object[] {});
            Assert.AreEqual("stuff", result.Value.ToString());
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void DuplicateIsInvoked() {
            RuntimeMember method = GetMethod("duplicate", 1);
            method.Invoke(new object[] {"stuff"});

            method = GetMethod("duplicate", 0);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("stuff", result.ToString());
        }

        [Test] public void QueryableMemberIsInvoked() {
            var queryable = new QueryableClass();
            RuntimeMember method = GetMethodFromProcessor(queryable, "dynamic", 1);
            TypedValue result = method.Invoke(new object[] {"stuff"});
            Assert.AreEqual("dynamicstuff", result.Value);
        }

        [Test]
        public void ExtensionMethodIsInvoked() {
            var name = new MemberName("increase(sampleextension)", "increase", typeof(SampleExtension));
            var method = MemberQuery.FindInstance(MemberQuery.FindMember, instance, new MemberSpecification(name, 1));
            var result = method.Invoke(new object[] {2});
            Assert.AreEqual(2, result.Value);
        }

        class QueryableClass: MemberQueryable {
            public RuntimeMember Find(MemberSpecification specification) {
                Assert.IsTrue(specification.MatchesIdentifierName("dynamic"));
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

                public Type ReturnType {
                    get { throw new NotImplementedException(); }
                }

                public string Name { get; private set; }
            }
        }

        [Test] public void MethodwithMisMatchedParameterNamesIsNotFound() {
            Assert.IsNull(MemberQuery.FindInstance(MemberQuery.FindMember, instance,
                new MemberSpecification("methodwithparms", 1).WithParameterNames(new [] {"garbage"})));
        }

        [Test] public void MethodwithMatchedParameterNamesIsFound() {
            Assert.IsNotNull(MemberQuery.FindInstance(MemberQuery.FindMember, instance,
                new MemberSpecification("methodwithparms", 1).WithParameterNames(new [] {"input"})));
        }
    }
}
