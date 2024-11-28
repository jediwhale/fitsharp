// Copyright © 2020 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fit.Test.Double;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseInterpreterTest{

        [Test] public void FixtureIsCreated() {
            var result = Parse("<table><tr><td>sample do</td></tr></table>");
            VerifyResult<SampleDoFixture>(result);
        }

        [Test] public void DomainClassIsWrapped() {
            var result = Parse("<table><tr><td>sample domain</td></tr></table>");
            var wrapper = VerifyResult<DefaultFlowInterpreter>(result);
            ClassicAssert.IsNotNull(wrapper.SystemUnderTest as SampleDomain);
        }

        [Test] public void FixtureWithSUTIsCreated() {
            var result = Parse("<table><tr><td>sample do</td></tr></table>", new TypedValue("target"));
            var sampleDo = VerifyResult<SampleDoFixture>(result);
            ClassicAssert.AreEqual("target", sampleDo.SystemUnderTest.ToString());
        }

        TypedValue Parse(string inputTables) {
            return Parse(inputTables, TypedValue.Void);
        }

        TypedValue Parse(string inputTables, TypedValue target) {
            processor = new Service.Service();
            processor.ApplicationUnderTest.AddAssembly(TargetFramework.Location(Assembly.GetExecutingAssembly()));
            processor.ApplicationUnderTest.AddNamespace(typeof(SampleDomain).Namespace);
            var parser = new ParseInterpreter { Processor = processor };
            var table = fit.Parse.ParseFrom(inputTables);
            return parser.Parse(typeof(Interpreter), target, table.Parts);
        }

         static T VerifyResult<T>(TypedValue result) where T: class {
            var anObject = result.Value as T;
            ClassicAssert.IsNotNull(anObject);
            return anObject;
        }

        CellProcessor processor;
    }
}
