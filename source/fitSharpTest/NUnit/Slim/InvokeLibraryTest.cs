// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;
using fitSharp.Test.Double.Slim;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class InvokeLibraryTest {
        [Test] public void SearchesLibraryInstancesForMethod() {
            var processor = Builder.Service();
            processor.PushLibraryInstance(new TypedValue(new SampleClass()));
            var runtime = new InvokeLibrary { Processor = processor };
            SampleClass.MethodCount = 0;
            runtime.Invoke(new TypedValue("stuff"), new MemberName("samplemethod"), new TreeList<string>());
            Assert.AreEqual(1, SampleClass.MethodCount);
        }
    }
}
