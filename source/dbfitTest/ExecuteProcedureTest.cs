using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using dbfit;
using dbfit.fixture;
using fit;
using fit.Service;
using fitSharp.Fit.Model;
using Moq;
using NUnit.Framework;

namespace dbfitTest.fixture {
    [TestFixture] public class ExecuteProcedureTest {
        private ExecuteProcedure fixture;
        private Mock<IDbEnvironment> db;

        [SetUp] public void SetUp() {
            db = new Mock<IDbEnvironment>();
            fixture = new ExecuteProcedure(db.Object, "myproc") {Processor = new Service()};
        }

        [Test]
        public void SortAccessorsInRightOrder()
        {
            //Prepare
            var accessorsToOrder = new DbParameterAccessor[4];
            accessorsToOrder[0]=new DbParameterAccessor(new SqlParameter(), typeof(string), 1, "String");
            accessorsToOrder[3]=new DbParameterAccessor(new SqlParameter(), typeof(string), 3, "String");
            accessorsToOrder[2]=new DbParameterAccessor(new SqlParameter(), typeof(string), 5, "String");
            accessorsToOrder[1]=new DbParameterAccessor(new SqlParameter(), typeof(string), 7, "String");
            
            //Execute
            DbParameterAccessor[] resultingAccessors = ExecuteProcedure.SortAccessors(accessorsToOrder);

            //Verify
            Assert.AreEqual(1, resultingAccessors[0].Position);
            Assert.AreEqual(3, resultingAccessors[1].Position);
            Assert.AreEqual(5, resultingAccessors[2].Position);
            Assert.AreEqual(7, resultingAccessors[3].Position);
        }
        [Test] public void ExecutesProcedureWithNoParameters() {
            var command = new TestCommand();
            RunTest(command, null, string.Empty);
            Assert.AreEqual(1, command.ExecuteNonQueryCalls);
        }

        [Test] public void ExecutesProcedureWithInputParameter() {
            var command = new TestCommand {
                NonQueryAction = (c => Assert.AreEqual("invalue", c.Parameters[0].Value))
            };

            RunTest(command, MakeParameters("inparm", ParameterDirection.Input),
                "<tr><td>inparm</td></tr><tr><td>invalue</td></tr>");

            Assert.AreEqual(1, command.ExecuteNonQueryCalls);
        }

        [Test] public void ExecutesProcedureWithOutputParameter() {
            var command = new TestCommand {
                NonQueryAction = (c => c.Parameters[0].Value = "outvalue")
            };

            RunTest(command, MakeParameters("outparm", ParameterDirection.Output),
                "<tr><td>outparm?</td></tr><tr><td>outvalue</td></tr>");

            Assert.AreEqual(1, command.ExecuteNonQueryCalls);
            Assert.AreEqual(1, fixture.TestStatus.Counts.GetCount(CellAttributes.RightStatus));
        }

        [Test] public void ExecutesProcedureWithInOutParameter() {
            var command = new TestCommand {
                NonQueryAction = (c => {
                    Assert.AreEqual("invalue", c.Parameters[0].Value);
                    c.Parameters[0].Value = "outvalue";
                })
            };

            RunTest(command, MakeParameters("ioparm", ParameterDirection.InputOutput),
                "<tr><td>ioparm</td><td>ioparm?</td></tr><tr><td>invalue</td><td>outvalue</td></tr>");

            Assert.AreEqual(1, command.ExecuteNonQueryCalls);
            Assert.AreEqual(1, fixture.TestStatus.Counts.GetCount(CellAttributes.RightStatus));
        }

        [Test] public void MarksWrongIfNoExpectedException() {
            fixture = new ExecuteProcedure(db.Object, "myproc", true) {Processor = new Service()};
            var command = new TestCommand();

            RunTest(command, MakeParameters("inparm", ParameterDirection.Input),
                "<tr><td>inparm</td></tr><tr><td>invalue</td></tr>");

            Assert.AreEqual(1, command.ExecuteNonQueryCalls);
            Assert.AreEqual(1, fixture.TestStatus.Counts.GetCount(CellAttributes.WrongStatus));
        }

        [Test] public void MarksRightIfExpectedException() {
            fixture = new ExecuteProcedure(db.Object, "myproc", true) {Processor = new Service()};
            var command = new TestCommand {
                NonQueryAction = (c => { throw new ApplicationException();})
            };

            RunTest(command, MakeParameters("inparm", ParameterDirection.Input),
                "<tr><td>inparm</td></tr><tr><td>invalue</td></tr>");

            Assert.AreEqual(1, command.ExecuteNonQueryCalls);
            Assert.AreEqual(1, fixture.TestStatus.Counts.GetCount(CellAttributes.RightStatus));
        }

        private static Dictionary<string, DbParameterAccessor> MakeParameters(string parameterName, ParameterDirection direction) {
            var parameter = new SqlParameter(parameterName, SqlDbType.VarChar, 10) {Direction = direction};
            return new Dictionary<string, DbParameterAccessor>
                   {{parameterName, new DbParameterAccessor(parameter, typeof (string), 0, "varchar")}};
        }

        private void RunTest(DbCommand command, Dictionary<string, DbParameterAccessor> parameters, string html) {
            db.Setup(d => d.CreateCommand("myproc", CommandType.StoredProcedure))
                .Returns(command);
            db.Setup(d => d.GetAllProcedureParameters("myproc"))
                .Returns(parameters);
            Parse table = new HtmlParser().Parse(string.Format("<table><tr><td>executeprocedure</td><td>myproc</td></tr>{0}</table>", html));
            fixture.DoTable(table);
        }
    }

    public class TestCommand: DbCommand {
        public readonly SqlCommand Command = new SqlCommand();
        public int ExecuteNonQueryCalls;
        public fitSharp.Machine.Extension.Action<TestCommand> NonQueryAction;

        public override void Prepare() {
            throw new System.NotImplementedException();
        }

        public override string CommandText {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override int CommandTimeout {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override CommandType CommandType {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override UpdateRowSource UpdatedRowSource {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        protected override DbConnection DbConnection {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        protected override DbParameterCollection DbParameterCollection {
            get { return Command.Parameters; }
        }

        protected override DbTransaction DbTransaction {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override bool DesignTimeVisible {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override void Cancel() {
            throw new System.NotImplementedException();
        }

        protected override DbParameter CreateDbParameter() {
            throw new System.NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            throw new System.NotImplementedException();
        }

        public override int ExecuteNonQuery() {
            ExecuteNonQueryCalls++;
            if (NonQueryAction != null) NonQueryAction(this);
            return 0;
        }

        public override object ExecuteScalar() {
            throw new System.NotImplementedException();
        }
    }
}
