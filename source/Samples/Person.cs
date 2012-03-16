// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;

namespace fitSharp.Samples
{
    public class Person
    {
        public int Id { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public static Person Parse(string name)
        {
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

        public override bool Equals(object obj)
        {
            var that = obj as Person;
            if (that == null)
                return false;
            return FirstName == that.FirstName && LastName == that.LastName;
        }

        public override int GetHashCode() {
            return Id.GetHashCode() + FirstName.GetHashCode() + LastName.GetHashCode();
        }

        public void SetTalented(bool talented)
        {
            IsTalented = talented;
        }

        public bool IsTalented { get; private set; }
    }
}
