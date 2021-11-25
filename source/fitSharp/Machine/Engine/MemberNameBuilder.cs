// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class MemberNameBuilder {
        public MemberNameBuilder(ApplicationUnderTest application) {
            this.application = application;
        }
        
        public MemberName MakeMemberName(string name) {
            if (name.EndsWith("?")) name = name.Substring(0, name.Length - 1);
            var ofPosition = name.IndexOf(" of ", StringComparison.OrdinalIgnoreCase);
            if (ofPosition < 0) ofPosition = name.IndexOf("_of_", StringComparison.OrdinalIgnoreCase);
            if (ofPosition > 0 && ofPosition < name.Length - 4) {
                var genericType = name.Substring(ofPosition + 4);
                var baseName = name.Substring(0, ofPosition);
                return new MemberName(name, baseName, MakeGenericTypes(new[] {genericType}));
            }
            var inPosition = name.IndexOf(" in ", StringComparison.OrdinalIgnoreCase);
            if (inPosition < 0) inPosition = name.IndexOf("_in_", StringComparison.OrdinalIgnoreCase);
            if (inPosition > 0 && inPosition < name.Length - 4) {
                var baseName = name.Substring(0, inPosition);
                var type = MakeType(name.Substring(inPosition + 4));
                return new MemberName(name, baseName, type);
            }
            return new MemberName(name);
        }

        IEnumerable<Type> MakeGenericTypes(IEnumerable<string> typeNames) {
            return typeNames.Select(MakeType);
        }

        Type MakeType(string name) {
            return application.FindType(new GracefulNameMatcher(name));
        }

        readonly ApplicationUnderTest application;
    }
}
