// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class MemberNameBuilder {
        public MemberNameBuilder(ApplicationUnderTest application) {
            this.application = application;
        }
        
        public MemberName MakeMemberName(string memberName) {
            var length = memberName.Length;
            if (memberName.EndsWith("?")) length--;
            var originalName = memberName.Substring(0, length);
            var name = originalName;
            var extensionType = Maybe<Type>.Nothing;
            var genericTypes = new List<Type>();
            
            var inPosition = name.LastIndexOf(extensionKeyword, StringComparison.OrdinalIgnoreCase);
            if (inPosition > 0 && inPosition < name.Length - 4) {
                extensionType = new Maybe<Type>(application.FindType(new GracefulNameMatcher(name.Substring(inPosition + 4))));
                name = name.Substring(0, inPosition);
            }
            
            var ofPosition = name.LastIndexOf(genericKeyword, StringComparison.OrdinalIgnoreCase);
            if (ofPosition > 0 && ofPosition < name.Length - 4) {
                genericTypes.Insert(0, application.FindType(new GracefulNameMatcher(name.Substring(ofPosition + 4))));
                name = name.Substring(0, ofPosition);
            }

            ofPosition = name.LastIndexOf(originalGenericKeyword, StringComparison.OrdinalIgnoreCase);
            if (ofPosition > 0 && ofPosition < name.Length - 4) {
                var type = MakeType(name.Substring(ofPosition + 4));
                var baseName = name.Substring(0, ofPosition);
                return type
                    .Select(t => new MemberName(name, baseName, new[] {t}))
                    .OrDefault(() => new MemberName(name));
            }
            return new MemberName(originalName, name, extensionType, genericTypes);
        }

        Maybe<Type> MakeType(string name) {
            return application.SearchTypes(new GracefulNameMatcher(name));
        }

        readonly ApplicationUnderTest application;
        
        static readonly string extensionKeyword = ".in.";
        static readonly string genericKeyword = ".of.";
        static readonly string originalGenericKeyword = " of ";
    }
}
