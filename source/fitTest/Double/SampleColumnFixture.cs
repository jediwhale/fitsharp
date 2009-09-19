using System;

namespace fit.Test.Double {
    public class SampleColumnFixture: ColumnFixture {
        public string Type;
        public DateTime Date { get { return DateTime.Now; } }
    }
}
