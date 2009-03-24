using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class ArrayFixtureUnderTestWithValueObjects: ArrayFixture {
        public ArrayFixtureUnderTestWithValueObjects():
            base(new object[]{
                new Pair(1,2),
                new Pair(3,4),
                new Pair(5,6)
            }) {}
        public class Pair {
            public MyClass one, two;
        		
            public Pair(int i, int j) {
                this.one = new MyClass(i);
                this.two = new MyClass(j);
            }
        }
        public class MyClass {
            private int i;

            public MyClass(int i) {
                this.i = i;
            }
            public static MyClass Parse(string s) {
                if (s.StartsWith("i "))
                    return new MyClass(int.Parse((s.Substring(2))));
                throw new ApplicationException("Invalid value: must start with 'i '");
            }
            public override string ToString() {
                return "i "+i;
            }
            public override bool Equals(object theOther) {
                if (!(theOther is MyClass))
                    return false;
                return ((MyClass)theOther).i == i;
            }
            public override int GetHashCode() {
                return i;
            }
        }
    }
}