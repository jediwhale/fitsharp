// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;
using fitSharp.Slim.Service;
using fitSharp.Test.Double.Slim;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ConverterTest {
        readonly Service processor = Builder.Service();

        [Test] public void CustomTypeIsParsed() {
            var converter = new CustomConverter {Processor = processor};
            ClassicAssert.IsTrue(converter.CanParse(typeof(CustomClass), TypedValue.Void, new TreeList<string>("info")));
            TypedValue parseResult = converter.Parse(typeof(CustomClass), TypedValue.Void, new TreeList<string>("info"));
            var result = parseResult.Value as CustomClass;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("custominfo", result.Info);
        }

        [Test] public void CustomTypeIsComposed() {
            var converter = new CustomConverter();
            ClassicAssert.IsTrue(converter.CanCompose(new TypedValue(new CustomClass {Info = "stuff"})));
            Tree<string> composeResult = converter.Compose(new TypedValue(new CustomClass {Info = "stuff"}));
            var result = composeResult.Value;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("mystuff", result);
        }

        class CustomConverter: Converter<CustomClass> {
            protected override CustomClass Parse(string input) {
                return new CustomClass {Info = ("custom" + input)};
            }

            protected override string Compose(CustomClass input) {
                return "my" + input.Info;
            }
        }

        class CustomClass {
            public string Info;
        }
    }
}