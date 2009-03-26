// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;
using fitSharp.Slim.Exception;
using fitSharp.Slim.Operators;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ComposeOperatorsTest {

        private Processor<string> processor;

        [SetUp] public void SetUp() {
            processor = new Processor<string>(new ApplicationUnderTest());
        }
        
        [Test] public void NullIsComposed() {
            CheckCompose(new ComposeDefault(), null, typeof (object), "null");
        }
        
        [Test] public void VoidIsComposed() {
            CheckCompose(new ComposeDefault(), null, typeof (void), "/__VOID__/");
        }
        
        [Test] public void DefaultComposeIsString() {
            CheckCompose(new ComposeDefault(), 1.23, typeof (double), "1.23");
        }
        
        [Test] public void BooleanTrueIsComposed() {
            CheckCompose(new ComposeBoolean(), true, typeof (bool), "true");
        }
        
        [Test] public void BooleanFalseIsComposed() {
            CheckCompose(new ComposeBoolean(), false, typeof (bool), "false");
        }

        [Test] public void ListIsComposedAsTree() {
            processor.AddOperator(new ComposeDefault());
            var result = Compose(new ComposeList(), new List<object> {"a", 1.23}, typeof (List<object>));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Branches.Count);
            Assert.AreEqual("a", result.Branches[0].Value); 
            Assert.AreEqual("1.23", result.Branches[1].Value); 
        }

        [Test] public void NestedListIsComposedAsTree() {
            processor.AddOperator(new ComposeDefault());
            processor.AddOperator(new ComposeList());
            var result = Compose(new ComposeList(), new List<object> {"a", new List<object> {"b", "c"}}, typeof (List<object>));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Branches.Count);
            Assert.AreEqual("a", result.Branches[0].Value);
            Assert.AreEqual("b", result.Branches[1].Branches[0].Value); 
            Assert.AreEqual("c", result.Branches[1].Branches[1].Value); 
        }

        [Test] public void ExceptionIsComposed() {
            CheckExceptionCompose(new ApplicationException("blah"), string.Empty);
        }

        [Test] public void MemberExceptionIsComposed() {
            CheckExceptionCompose(new MemberMissingException(typeof(string), "garbage", 0), "message<<NO_METHOD_IN_CLASS garbage System.String>> ");
        }

        [Test] public void TypeExceptionIsComposed() {
            CheckExceptionCompose(new TypeMissingException("garbage", "and stuff"), "message<<NO_CLASS garbage>> ");
        }

        [Test] public void ConstructorExceptionIsComposed() {
            CheckExceptionCompose(new ConstructorMissingException(typeof(string), 0), "message<<NO_CONSTRUCTOR System.String>> ");
        }

        [Test] public void CreateExceptionIsComposed() {
            CheckExceptionCompose(new CreateException(typeof(string), 0, new ApplicationException("blah")), "message<<COULD_NOT_INVOKE_CONSTRUCTOR System.String>> ");
        }

        [Test] public void MemoryExceptionIsComposed() {
            CheckExceptionCompose(new MemoryMissingException<SavedInstance>(new SavedInstance("stuff")), "message<<NO_INSTANCE stuff>> ");
        }

        [Test] public void InstructionExceptionIsComposed() {
            CheckExceptionCompose(new InstructionException(new TreeList<string>().AddBranchValue("stuff").AddBranchValue("nonsense")),
                                  "message<<MALFORMED_INSTRUCTION stuff,nonsense>> ");
        }

        [Test] public void ParseExceptionIsComposed() {
            CheckExceptionCompose(new ParseException<string>("member", typeof(string), 0, "garbage", new ApplicationException("blah")),
                                  "message<<NO_CONVERTER_FOR_ARGUMENT_NUMBER System.String>> ");
        }

        private Tree<string> Compose(ComposeOperator<string> composeOperator, object instance, Type type) {
            Tree<string> result = null;
            Assert.IsTrue(composeOperator.TryCompose(processor, new TypedValue(instance, type), ref result));
            return result;
        }

        private void CheckExceptionCompose(Exception exception, string expected) {
            CheckCompose(new ComposeException(), exception, exception.GetType(), string.Format("__EXCEPTION__:{0}{1}", expected, exception));
        }

        private void CheckCompose(ComposeOperator<string> composeOperator, object instance, Type type, object expected) {
            Tree<string> result = null;
            Assert.IsTrue(composeOperator.TryCompose(processor, new TypedValue(instance, type), ref result));
            Assert.AreEqual(expected, result.Value);
        }
    }
}