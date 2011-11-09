// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Exception {
    public class MemberMissingException: ValidationException {
        public Type Type { get; private set; }
        public string MemberName { get; private set; }
        public int ParameterCount { get; private set; }

        public MemberMissingException(Type type, string memberName, int parameterCount)
            : base(string.Format("Member '{1}' with {2} parameter(s) not found for type '{0}'.", type.FullName, memberName, parameterCount)) {
            Type = type;
            MemberName = memberName;
            ParameterCount = parameterCount;
        }
    }
}