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
            var originalName = memberName.EndsWith("?") ? memberName.Substring(0, memberName.Length - 1) : memberName;
            name = originalName;
            var extensionType = Maybe<Type>.Nothing;
            var genericTypes = new List<Type>();

            ProcessKeyword(extensionKeyword,
                matcher => { extensionType = Maybe<Type>.Of(application.FindType(matcher)); });

            while (ProcessKeyword(genericKeyword,
                matcher => { genericTypes.Insert(0, application.FindType(matcher)); })) {
            }

            // old behaviour, preserve for compatibility
            var ofPosition = name.LastIndexOf(originalGenericKeyword, StringComparison.OrdinalIgnoreCase);
            if (ofPosition > 0 && ofPosition < name.Length - 4) {
                var type = MakeType(name.Substring(ofPosition + 4));
                var baseName = name.Substring(0, ofPosition);
                return type
                    .Select(t => new MemberName(name, baseName, new[] {t}))
                    .OrElseGet(() => new MemberName(name));
            }
            
            return new MemberName(originalName, name, extensionType, genericTypes);
        }

        Maybe<Type> MakeType(string typeName) {
            return application.SearchTypes(new GracefulNameMatcher(typeName));
        }

        bool ProcessKeyword(string keyword, Action<NameMatcher> action) {
            var position = name.LastIndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (position <= 0 || position >= name.Length - keyword.Length) return false;
            action(new GracefulNameMatcher(name.Substring(position + keyword.Length)));
            name = name.Substring(0, position);
            return true;
        }

        readonly ApplicationUnderTest application;
        string name;

        const string extensionKeyword = ".in.";
        const string genericKeyword = ".of.";
        const string originalGenericKeyword = " of ";
    }
}
