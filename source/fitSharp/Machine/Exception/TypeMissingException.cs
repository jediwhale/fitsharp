// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Runtime.Serialization;

namespace fitSharp.Machine.Exception {
    [Serializable]
    public class TypeMissingException: ApplicationException {
        public string TypeName { get; private set; }

        public TypeMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}

        public TypeMissingException(string typeName, string message)
            : base(string.Format("Type '{0}' not found in assemblies:{1}{2}", typeName, Environment.NewLine, message)) {
            TypeName = typeName;
        }
        
    }
}