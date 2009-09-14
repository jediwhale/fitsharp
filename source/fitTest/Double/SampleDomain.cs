// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Test.NUnit;

namespace fit.Test.Double {
    public class SampleDomain {
        public SampleDomain() {}
        public SampleDomain(string theName) {
            Name = theName;
        }
        public string Name;
        public string Message;
        public int IntegerField;
        public int another_field;
        public string StringField;
        public DateTime DateTimeField;
        public Person PersonField;

        public static string StaticMethod() {
            return "hello";
        }

        public string ThrowException() {
            if (Message == "OK") return Message;
            if (Message == null) throw new NullReferenceException();
            else throw new ApplicationException(Message);
        }

        public void AddDays(int days) { DateTimeField = DateTimeField.AddDays(days); }
    }

    public class another_sample_domain {}
}