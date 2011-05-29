namespace dbfit
{
    public class SybaseTest: DatabaseTest
    {
        public SybaseTest()
            : base(new SybaseEnvironment())
        {

        }
    }
}