// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Exception {
    public class ParseException<T>: ApplicationException {
        public T Subject { get; private set; }
        public string MemberName { get; private set; }
        public int Index { get; private set; }
        public Type Type { get; private set; }

        public ParseException(string memberName, Type type, int index, T subject, System.Exception inner)
            : base(string.Format("Parse parameter {1} for '{0}' type {2} failed.", memberName, index, type), inner) {
            Subject = subject;
            MemberName = memberName;
            Type = type;
            Index = index;
        }
    }
}