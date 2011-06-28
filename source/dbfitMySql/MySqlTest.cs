namespace dbfit
{
    public class MySqlTest : DatabaseTest
    {
        public MySqlTest()
            : base(new MySqlEnvironment())
        {

        }
    }
}
