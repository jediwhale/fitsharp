// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class OperatorsTest {

        [Test] public void FindsDefaultOperator() {
            var operators = new Operators<string, Processor<string>>();
            var added = operators.Add(typeof(SampleDefaultCreate).FullName);
            var result = operators.FindOperator<CreateOperator<string>>(new object[] {new IdentifierName("test"), null});
            ClassicAssert.AreSame(added, result);
        }

        [Test]
        public void ReplacesOperator() {
            var operators = new Operators<string, Processor<string>>();
            operators.Replace(nameof (CreateDefault<string, Processor<string>>), typeof(SampleDefaultCreate).FullName);
            var result = operators.FindOperator<CreateOperator<string>>(new object[] {new IdentifierName("test"), null});
            ClassicAssert.AreEqual(typeof(SampleDefaultCreate), result.GetType());
        }
    }
    
    public class SampleDefaultCreate: Operator<string, Processor<string>>, CreateOperator<string> {
        public bool CanCreate(NameMatcher memberName, Tree<string> parameters) { return true; }
        public TypedValue Create(NameMatcher memberName, Tree<string> parameters) { return TypedValue.Void; }
    }
}
