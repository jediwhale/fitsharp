// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Exception {
    public class TypeMissingException: ApplicationException {
        public string TypeName { get; private set; }

        public TypeMissingException(string typeName, string message)
            : base(string.Format("Type '{0}' not found in assemblies:\n{1}", typeName, message)) {
            TypeName = typeName;
        }
        
    }
}