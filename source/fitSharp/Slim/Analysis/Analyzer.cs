// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Analysis {
    public class Analyzer {
        public Analyzer() : this(new ApplicationUnderTest()) {}

        public Analyzer(ApplicationUnderTest applicationUnderTest) {
            this.applicationUnderTest = applicationUnderTest;
            actions.Add("make", ProcessMake);
            actions.Add("import", ProcessImport);
            actions.Add("call", ProcessCall);
            actions.Add("callAndAssign", ProcessCallAndAssign);
            actions.Add("assign", i => {});
        }
        
        public void Process(SlimTree instruction) {
            actions[instruction.ValueAt(1)](instruction);
        }
        
        public IEnumerable<string> Calls => calls;

        void ProcessCall(SlimTree instruction) {
            AnalyzeCall(instruction, 0);
        }

        void ProcessCallAndAssign(SlimTree instruction) {
            AnalyzeCall(instruction, 1);
        }
        
        void AnalyzeCall(SlimTree instruction, int offset) {
            var instanceText = instruction.ValueAt(offset + 2);
            var memberText = instruction.ValueAt(offset + 3);
            var memberName = new MemberNameBuilder(applicationUnderTest).MakeMemberName(memberText);
            if (!instances.ContainsKey(instanceText)) {
                calls.Add($"* Unknown type for method '{memberText}' *");
                return;
            }
            var parameterCount = instruction.Branches.Count - 4 - offset;
            var specification = new MemberSpecification(memberName, parameterCount);
            var member = MemberQuery.FindInstance(FindMember, new TypedValue(null, instances[instanceText]), specification);
            if (!member.IsPresent) {
                foreach (var type in library) {
                    member = MemberQuery.FindInstance(FindMember, new TypedValue(null, type), specification);
                    if (member.IsPresent) break;
                }
            }
            calls.Add(member.Select(m => m.Name).OrElse($"* Method '{memberText}' not found *"));
        }

        static TypedValue FindMember(TypedValue instance, MemberQuery query) {
            return query.FindMember(instance).TypedValue;
        }
        
        void ProcessMake(SlimTree instruction) {
            var typeText = instruction.ValueAt(3);
            var type = applicationUnderTest.SearchTypes(new GracefulNameMatcher(typeText));
            var instanceText = instruction.ValueAt(2);
            type.IfPresent(t => {
                if (instanceText.StartsWith("library")) {
                    library.Push(t);
                }
                else {
                    instances[instanceText] = t;
                }
            });
            calls.Add(
                type.Select(t => $"{t.FullName}:{t.Name}({instruction.Branches.Count - 4})")
                    .OrElse($"* Type '{typeText}' not found *"));
        }

        void ProcessImport(SlimTree instruction) {
            applicationUnderTest.AddNamespace(instruction.ValueAt(2));
        }

        readonly Dictionary<string, Action<SlimTree>> actions = new Dictionary<string, Action<SlimTree>>();
        readonly ApplicationUnderTest applicationUnderTest;
        readonly Dictionary<string, Type> instances = new Dictionary<string, Type>();
        readonly Stack<Type> library = new Stack<Type>();
        readonly List<string> calls = new List<string>();
    }
}