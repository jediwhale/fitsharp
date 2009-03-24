namespace fitSharp.Fit.Application {
    public interface Configurable {
        void Remove(string value);
        void Add(string value);
        void Insert(string value, string position);
    }
}
