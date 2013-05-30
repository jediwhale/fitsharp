// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class MemberName {
        public static readonly MemberName Constructor = new MemberName("\".ctor\"");
        public static readonly MemberName ParseMethod = new MemberName("Parse");
        public static readonly MemberName SetUp = new MemberName("setup");
        public static readonly MemberName TearDown = new MemberName("teardown");

        public MemberName(string name): this(name, name, new Type[] {}) {}

        public MemberName(string name, string baseName, IEnumerable<Type> genericTypes) {
            OriginalName = name;
            Name = new GracefulName(name).IdentifierName.ToString();
            BaseName = new GracefulName(baseName).IdentifierName.ToString();
            GenericTypes = genericTypes;
        }

        public MemberName WithNamedParameters() {
            HasNamedParameters = true;
            return this;
        }

        public string Name { get; private set; }
        public string OriginalName { get; private set; }
        public string BaseName { get; private set; }
        public IEnumerable<Type> GenericTypes { get; private set; }
        public bool HasNamedParameters { get; private set; }
        public override string ToString() { return Name; }
    }
}
