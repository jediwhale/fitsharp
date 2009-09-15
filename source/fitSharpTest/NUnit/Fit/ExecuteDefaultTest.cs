// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ExecuteDefaultTest {
        [Test] public void MethodIsInvoked() {
            var processor = new Mock<CellProcessor>();
            var execute = new ExecuteDefault {Processor = processor.Object};
            var target = new TypedValue("target");
            var result = new TypedValue("result");
            var parameters = new ExecuteParameters(
                new ExecuteContext(target),
                ExecuteParameters.MakeInvoke(new StringCell("member"), new TreeList<Cell>()));
            processor
                .Setup(p => p.Parse(typeof (MemberName), It.IsAny<TypedValue>(), It.Is<StringCell>(c => c.Text == "member")))
                .Returns(new TypedValue(new MemberName("member")));
            processor
                .Setup(p => p.Invoke(target, "member", It.Is<Tree<Cell>>(c => c.Branches.Count == 0)))
                .Returns(result);
            Assert.AreEqual(result, execute.Execute(parameters));
        }

        [Test] public void ProcedureIsInvoked() {
            var processor = new Mock<CellProcessor>();
            var execute = new ExecuteDefault {Processor = processor.Object};
            var target = new TypedValue("target");
            var result = new TypedValue("result");
            var procedure = new Procedure("procedure",
                                          new TreeList<Cell>().AddBranch(
                                              new TreeList<Cell>().AddBranch(new StringCell("member"))));
            var parameters = new ExecuteParameters(
                new ExecuteContext(target),
                ExecuteParameters.MakeInvoke(new StringCell("procedure"), new TreeList<Cell>()));
            processor
                .Setup(p => p.Parse(typeof (MemberName), It.IsAny<TypedValue>(), It.Is<StringCell>(c => c.Text == "procedure")))
                .Returns(new TypedValue(new MemberName("procedure")));
            processor
                .Setup(p => p.Load(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(procedure);
            execute.Execute(parameters);
        }
    }
}
