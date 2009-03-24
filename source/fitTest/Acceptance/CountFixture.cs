namespace fit.Test.Acceptance
{
    public class CountFixture : Fixture
    {
        private int counter = 0;

        public void Count()
        {
            counter++;
        }

        public int Counter
        {
            set { counter = value; }
            get { return counter; }
        }
    }
}
