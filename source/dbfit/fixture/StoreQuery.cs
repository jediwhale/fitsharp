using System;
using fit;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace dbfit.fixture
{
    public class StoreQuery:fit.Fixture
    {
        private IDbEnvironment dbEnvironment;
        private String query;
        private String symbolName;
        public StoreQuery()
        {
            dbEnvironment = DbEnvironmentFactory.DefaultEnvironment;
        }
        public StoreQuery(IDbEnvironment environment, String query, String symbolName)
        {
            this.dbEnvironment = environment;
            this.query = query;
            this.symbolName = symbolName;
        }

        public override void DoTable(Parse table)
        {
            if (query == null || symbolName == null)
            {
                if (Args.Length < 2) throw new ApplicationException("No query and symbol name specified to StoreQuery constructor or argument list");
                query = Args[0];
                symbolName = Args[1];
            }
            if (symbolName.StartsWith(">>")) symbolName = symbolName.Substring(2);
            Processor.Get<Symbols>().Save(symbolName, DatabaseTest.GetDataTable(Processor, query,dbEnvironment));
        }

    }
}
