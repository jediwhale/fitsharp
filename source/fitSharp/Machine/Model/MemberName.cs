// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fitSharp.Machine.Engine;

namespace fitSharp.Machine.Model {
    //MemberName is a bit misleading, it carries other stuff too
    //everything we need along with instance and parameters to do invoke
    public class MemberName {
        public static readonly MemberName Constructor = new MemberName("\".ctor\"");
        public static readonly MemberName ParseMethod = new MemberName("Parse");
        public static readonly MemberName SetUp = new MemberName("setup");
        public static readonly MemberName TearDown = new MemberName("teardown");

        public MemberName(string name): this(name, name, new Type[] {}) {}

        public MemberName(string name, string baseName, IEnumerable<Type> genericTypes) {
            OriginalName = name;
            Name = new GracefulName(name).IdentifierName.ToString(); 
            this.baseName = new GracefulName(baseName).IdentifierName.ToString(); 
            this.genericTypes = genericTypes;
        }

        public MemberName(string originalName, string baseName, Type extensionType) {
            OriginalName = originalName;
            this.baseName = new GracefulName(baseName).IdentifierName.ToString();
            Name = this.baseName;
            this.extensionType = extensionType;
        }
        
        public MemberName WithNamedParameters() { 
            HasNamedParameters = true;
            return this;
        }

        public bool Matches(MethodInfo info) {
            return (info.IsGenericMethod && genericTypes != null && genericTypes.Any()) || extensionType != null
                ? MatchesBaseName(info.Name) 
                : MatchesGetSetName(info.Name);
        }
        
        public RuntimeMember MakeMember(MethodInfo info, object instance) {
            return info.IsGenericMethod
                ? new MethodMember(MakeGenericMethod(info), instance)
                : new MethodMember(info, instance);
        }
        
        bool MatchesBaseName(string name) {
            return new IdentifierName(baseName).Matches(name);
        }
       
        public bool MatchesGetSetName(string name) {
            var identifier = new IdentifierName(Name);
            if (identifier.Matches(name)) return true;
            if (!identifier.MatchName.StartsWith("set") && !identifier.MatchName.StartsWith("get")) return false;
            return new IdentifierName(identifier.MatchName.Substring(3)).Matches(name);
        }
        
        MethodInfo MakeGenericMethod(MethodInfo info) {
            return info.MakeGenericMethod(genericTypes.ToArray());
        }

        public Maybe<RuntimeMember> FindMatchingMember(MemberQuery query, object instance) {
            return extensionType == null
                ? query.FindInstanceMember(instance)
                : query.FindExtensionMember(extensionType, instance);
        }

        public string Name { get; }
        public string OriginalName { get; }
        public bool HasNamedParameters { get; private set; }
        public override string ToString() { return baseName; }
        
        readonly IEnumerable<Type> genericTypes;
        readonly Type extensionType;
        readonly string baseName;
    }
}
