// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Service {
    public class Analyzer {
        public Analyzer() {
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
            var instance = new TypedValue(null, instances[instanceText]);
            var member = MemberQuery.FindInstance(FindMember, instance,
                new MemberSpecification(memberName, parameterCount));
            calls.Add(member.Select(m => m.Name).OrElse($"* Method '{memberText}' not found *"));
        }

        static TypedValue FindMember(TypedValue instance, MemberQuery query) {
            return query.FindMember(instance).TypedValue;
        }
        
        void ProcessMake(SlimTree instruction) {
            var typeText = instruction.ValueAt(3);
            var type = applicationUnderTest.SearchTypes(new GracefulNameMatcher(typeText));
            type.IfPresent(t => {instances[instruction.ValueAt(2)] = t;});
            calls.Add(
                type.Select(t => $"{t.FullName}:{t.Name}({instruction.Branches.Count - 4})")
                    .OrElse($"* Type '{typeText}' not found *"));
        }

        void ProcessImport(SlimTree instruction) {
            applicationUnderTest.AddNamespace(instruction.ValueAt(2));
        }

        readonly Dictionary<string, Action<SlimTree>> actions = new Dictionary<string, Action<SlimTree>>();
        readonly ApplicationUnderTest applicationUnderTest = new ApplicationUnderTest();
        readonly Dictionary<string, Type> instances = new Dictionary<string, Type>();
        readonly List<string> calls = new List<string>();
    }
}