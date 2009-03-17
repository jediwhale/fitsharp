using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ConverterTest {
        private readonly Processor<string> processor = new Processor<string>(new ApplicationUnderTest());

        [Test] public void CustomTypeIsParsed() {
            var converter = new CustomConverter();
            TypedValue parseResult = TypedValue.Void;
            Assert.IsTrue(converter.TryParse(processor, typeof(CustomClass), TypedValue.Void, new TreeLeaf<string>("info"), ref parseResult));
            var result = parseResult.Value as CustomClass;
            Assert.IsNotNull(result);
            Assert.AreEqual("custominfo", result.Info);
        }

        [Test] public void CustomTypeIsComposed() {
            var converter = new CustomConverter();
            Tree<string> composeResult = null;
            Assert.IsTrue(converter.TryCompose(processor, new TypedValue(new CustomClass {Info = "stuff"}), ref composeResult));
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