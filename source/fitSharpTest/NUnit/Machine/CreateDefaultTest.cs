// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class CreateDefaultTest {
        private CreateDefault<string, BasicProcessor> runtime;
        private readonly BasicProcessor processor = new BasicProcessor();
        [SetUp] public void SetUp() {
            runtime = new CreateDefault<string, BasicProcessor> { Processor = processor};
        }

        [Test] public void InstanceIsCreated() {
            TypedValue result = runtime.Create(new IdentifierName(typeof(SampleClass).FullName), new TreeList<string>());
            Assert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void StandardInstanceIsCreated() {
            TypedValue result = runtime.Create(new IdentifierName("System.Boolean"), new TreeList<string>());
            Assert.IsTrue(result.Value is bool);
        }
    }
}
