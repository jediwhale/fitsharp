// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml;
using fitlibrary.tree;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleDomain {
        public SampleDomain() {}
        public SampleDomain(string theName) {
            Name = theName;
        }
        public string Message;
        public bool BooleanField;
        public int IntegerField;
        public int another_field;
        public string StringField;
        public DateTime DateTimeField;
        public Guid GuidField;
        public Calendar Calendar;
        public Person PersonField;
        public Tree TreeField;
        public string NewName;
        public string[] Strings;

        public string Name {
            get {
                Log.Write("get_Name()");
                return name;
            }
            set {
                name = value;
                Log.Write("set_Name(" + name + ")");
            }
        }

        public int NameLength { get { return Name.Length; } }

        public char NameAt(int index) {
            return Name[index];
        }

        public static string StaticMethod() {
            return "hello";
        }

        public string StringsAt(int index) {
            return Strings[index];
        }

        public bool Check(int x, int y) {
            return x == y;
        }

        public void NameFromFirstAndLast(string first, string last) {
            Name = first + " " + last;
        }

        public string ThrowException() {
            if (Message == "OK") return Message;
            if (Message == null) throw new NullReferenceException();
            throw new ApplicationException(Message);
        }

        public string Throw(string message) {
            if (message == null) throw new NullReferenceException();
            if (message == "OK") return message;
            throw new ApplicationException(message);
        }

        public void AddDays(int days) { DateTimeField = DateTimeField.AddDays(days); }

        public Tree MakeTree(string[] leaves) {
            return new SampleTree(string.Empty, leaves.Length == 1 && leaves[0].Length == 0 ? new string[] {} : leaves);
        }

        public Person MakePersonWithFirstAndLast(string first, string last) {
            return new Person(first, last);
        }

        public IEnumerable<Person> GetPeople(string[] names) {
            return names.Select(n => {
                var split = n.Split('-');
                return new Person(split[0], split[1]);
            });
        }

        public Dictionary<string, Person> MakeDictionary() {
            return new Dictionary<string, Person> {{"key1", new Person("Bob", "Martin")}, {"key2", new Person("Mike", "Stockdale")}};
        }

        public IEnumerable<KeyValuePair<string, string>> MakeKeyValues() {
            var dictionary = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
            return dictionary.AsEnumerable().OrderBy(p => p.Key);
        }

        public DataTable MakeDataTable() {
            var table = new DataTable();
            table.Columns.Add("column1", typeof (string));
            table.Columns.Add("column2", typeof (string));
            var row = table.NewRow();
            row["column1"] = "value1";
            row["column2"] = "value2";
            table.Rows.Add(row);
            return table;
        }

        public XmlDocument MakeXml() {
            var document = new XmlDocument();
            document.LoadXml("<root><child>text</child></root>");
            return document;
        }

        string name;
    }

    public class another_sample_domain {}
}
