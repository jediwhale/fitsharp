using System.Linq;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class DeepCopyTest {

        [Test]
        public void CopiesLeaf() {
            Assert.AreEqual("leaf[]", Write(copy.Make(new CellTreeLeaf("leaf"))));
        }

        [Test]
        public void CopiesBranches() {
            var source = new CellTreeLeaf("root");
            source.AddBranchValue(new CellTreeLeaf("leafa"));
            source.AddBranchValue(new CellTreeLeaf("leafb"));
            var result = copy.Make(source);
            source.Branches[0].Add(new CellTreeLeaf("extra"));
            Assert.AreEqual("root[leafa[];leafb[]]", Write(result));
        }

        [Test]
        public void CopiesSubstitutes() {
            var source = new CellTreeLeaf("root");
            source.AddBranchValue(new CellTreeLeaf("leafa"));
            source.AddBranchValue(new CellTreeLeaf("leafb"));
            var result = copy.Make(source, original => original.Value != null && original.Value.Text == "leafa"
                ? new CellTree("new", "ones")
                : null);
            source.Branches[0].Add(new CellTreeLeaf("extra"));
            Assert.AreEqual("root[null[new[];ones[]];leafb[]]", Write(result));
        }

        [Test]
        public void DoesNotSubstituteWithinSubstitutes() {
            var source = new CellTreeLeaf("root");
            source.AddBranchValue(new CellTreeLeaf("leafa"));
            source.AddBranchValue(new CellTreeLeaf("leafb"));
            var result = copy.Make(source, original => original.Value != null && original.Value.Text == "leafa"
                ? new CellTree("new", "leafa")
                : null);
            source.Branches[0].Add(new CellTreeLeaf("extra"));
            Assert.AreEqual("root[null[new[];leafa[]];leafb[]]", Write(result));
        }

        [Test]
        public void CopiesAttributes() {
            var source = new CellTreeLeaf("leaf");
            source.Value.SetAttribute(CellAttribute.Label, "mylabel");
            var result = copy.Make(source);
            source.Value.SetAttribute(CellAttribute.Label, "other");
            Assert.AreEqual("leaf,Label:mylabel[]", Write(result));
        }

        [SetUp]
        public void SetUp() {
            copy = new DeepCopy(Builder.CellProcessor());
        }

        static string Write(Tree<Cell> input) {
            var result = new StringBuilder();
            if (input.Value == null) {
                result.Append("null");
            }
            else {
                result.Append(input.Value.Text);
                foreach (var pair in input.Value.Attributes) {
                    result.Append("," + pair.Key + ":" + pair.Value.Value);
                }
            }
            result.Append("[" + string.Join(";", input.Branches.Select(Write)) + "]");
            return result.ToString();
        }
         
        DeepCopy copy;
    }
}
