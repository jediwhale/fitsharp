// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class RuntimeTypeTest {
        private SampleClass instance;

        [SetUp] public void SetUp() {
            instance = new SampleClass();
        }

        [Test] public void VoidMethodIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "voidmethod", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);
        }

        [Test] public void MethodWithReturnIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "methodnoparms", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("samplereturn", result.Value.ToString());
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void MethodWithUnderscoresIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "methodwithunderscores", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("samplereturn", result.Value.ToString());
        }

        [Test] public void MethodWithParmsIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "methodwithparms", 1);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"input"});
            Assert.AreEqual("sampleinput", result.Value.ToString());
        }

        [Test] public void StaticMethodWithParmsIsInvoked() {
            RuntimeMember method = new RuntimeType(instance.GetType()).FindStatic("parse", new [] {typeof(string)});
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"input"});
            Assert.AreEqual(typeof(SampleClass), result.Type);
        }

        [Test] public void ConstructorIsInvoked() {
            RuntimeMember method = new RuntimeType(instance.GetType()).GetConstructor(0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual(typeof(SampleClass), result.Type);
        }

        [Test] public void PropertySetAndGetIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "property", 1);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"stuff"});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);

            method = RuntimeType.GetInstance(new TypedValue(instance), "property", 0);
            Assert.IsNotNull(method);
            result = method.Invoke(new object[] {});
            Assert.AreEqual("stuff", result.Value.ToString());
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void IndexerIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "anything", 0);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("indexanything", result.Value);
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void FieldIsInvokedWithGetAndSet() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "setfield", 1);
            Assert.IsNotNull(method);
            TypedValue result = method.Invoke(new object[] {"stuff"});
            Assert.AreEqual(null, result.Value);
            Assert.AreEqual(typeof(void), result.Type);

            method = RuntimeType.GetInstance(new TypedValue(instance), "getfield", 0);
            Assert.IsNotNull(method);
            result = method.Invoke(new object[] {});
            Assert.AreEqual("stuff", result.Value.ToString());
            Assert.AreEqual(typeof(string), result.Type);
        }

        [Test] public void DuplicateIsInvoked() {
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(instance), "duplicate", 1);
            method.Invoke(new object[] {"stuff"});

            method = RuntimeType.GetInstance(new TypedValue(instance), "duplicate", 0);
            TypedValue result = method.Invoke(new object[] {});
            Assert.AreEqual("stuff", result.ToString());
        }

        [Test] public void QueryableMemberIsInvoked() {
            var queryable = new QueryableClass();
            RuntimeMember method = RuntimeType.GetInstance(new TypedValue(queryable), "dynamic", 1);
            TypedValue result = method.Invoke(new object[] {"stuff"});
            Assert.AreEqual("dynamicstuff", result.Value);
        }

        private class QueryableClass: MemberQueryable {
            public RuntimeMember Find(IdentifierName memberName, int parameterCount, Type[] parameterTypes) {
                return new QueryableMember(memberName.ToString());
            }

            private class QueryableMember: RuntimeMember {
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

                public Type ReturnType {
                    get { throw new NotImplementedException(); }
                }

                public string Name { get; private set; }
            }

        }

    }
}
