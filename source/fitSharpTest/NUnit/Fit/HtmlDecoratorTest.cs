using fitSharp.Fit.Runner;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class HtmlDecoratorTest {

        [Test]
        public void AddsTextToStart() {
            Assert.AreEqual("newtextexistingtext", HtmlDecorator.AddToStart("newtext", "existingtext"));
        }

        [Test, Ignore]
        public void AddsTextAfterDocType() {
            Assert.AreEqual("<!doctype stuff>newtextexistingtext", HtmlDecorator.AddToStart("newtext", "<!doctype stuff>existingtext"));
        }
         
    }
}
