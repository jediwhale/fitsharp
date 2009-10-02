// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ConverterTest {
        private readonly Service processor = new Service();

        [Test] public void CustomTypeIsParsed() {
            var converter = new CustomConverter {Processor = processor};
            Assert.IsTrue(converter.CanParse(typeof(CustomClass), TypedValue.Void, new TreeList<string>("info")));
            TypedValue parseResult = converter.Parse(typeof(CustomClass), TypedValue.Void, new TreeList<string>("info"));
            var result = parseResult.Value as CustomClass;
            Assert.IsNotNull(result);
            Assert.AreEqual("custominfo", result.Info);
        }

        [Test] public void CustomTypeIsComposed() {
            var converter = new CustomConverter();
            Assert.IsTrue(converter.CanCompose(new TypedValue(new CustomClass {Info = "stuff"})));
            Tree<string> composeResult = converter.Compose(new TypedValue(new CustomClass {Info = "stuff"}));
            var result = composeResult.Value;
            Assert.IsNotNull(result);
            Assert.AreEqual("mystuff", result);
        }

        private class CustomConverter: Converter<CustomClass> {
            protected override CustomClass Parse(string input) {
                return new CustomClass {Info = ("custom" + input)};
            }

            protected override string Compose(CustomClass input) {
                return "my" + input.Info;
            }
        }

        private class CustomClass {
            public string Info;
        }
    }
}