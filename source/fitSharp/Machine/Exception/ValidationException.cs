// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Runtime.Serialization;

namespace fitSharp.Machine.Exception {
    //todo: use return not exception
    public class ValidationException: ApplicationException {
        public ValidationException() {}

        public ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}

        public ValidationException(string message) : base(message) {}

        public ValidationException(string message, System.Exception innerException) : base(message, innerException) {}
    }
}
