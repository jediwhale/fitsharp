// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;

namespace fitSharp.Samples {
    public class Person {
        public int Id { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public double Height;
        public void Grow(double growth) {
            Height += growth;
        }

        public static Person Parse(string name) {
            var names = name.Split(' ');
            return new Person(names[0], names[1]);
        }

        public Person(string firstName, string lastName) {
            FirstName = firstName;
            LastName = lastName;
        }

        public Person(int id, string firstName, string lastName) {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString() {
            var builder = new StringBuilder(FirstName);
            if (builder.Length > 0 && !string.IsNullOrEmpty(LastName))
            {
                builder.Append(" ");
            }
            return builder.Append(LastName).ToString();
        }

        public override bool Equals(object obj) {
            var that = obj as Person;
            if (that == null)
                return false;
            return FirstName == that.FirstName && LastName == that.LastName;
        }

        public override int GetHashCode() {
            return Id.GetHashCode() + FirstName.GetHashCode() + LastName.GetHashCode();
        }

        public void SetTalented(bool talented) {
            IsTalented = talented;
        }

        public bool IsTalented { get; private set; }

        public Name FullName { get { return new Name{First = FirstName, Last = LastName}; } }

        public class Name {
            public string First;
            public string Last;
        }
    }
}
