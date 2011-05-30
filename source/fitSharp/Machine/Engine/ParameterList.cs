// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using System.Linq;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine
{
    public class ParameterList<T> {
        public ParameterList(Processor<T> processor) {
            this.processor = processor;
        }

        public object[] GetParameterList(TypedValue instance, Tree<T> parameters, RuntimeMember member) {
            return parameters.Branches.Aggregate(new List<object>(),
                (parameterList, parameter) => {
                     parameterList.Add(ParseParameterValue(member, instance, parameter, parameterList.Count));
                    return parameterList;
            }).ToArray();
        }

        public object[] GetNamedParameterList(TypedValue instance, Tree<T> parameters, RuntimeMember member, IList<string> parameterNames) {
            var result = new object[parameterNames.Count];

            for (int i = 0; i < parameterNames.Count; i++) {
                for (int j = 0; j < parameterNames.Count; j++) {
                    var parameterNameId = new IdentifierName(parameterNames[j]);
                    if (!parameterNameId.Matches(member.GetParameterName(i))) continue;
                    result[i] = ParseParameterValue(member, instance, parameters.Branches[2*j + 1], i);
                }
            }
            return result;
        }

        readonly Processor<T> processor;

        object ParseParameterValue(RuntimeMember member, TypedValue instance, Tree<T> parameter, int parameterIndex) {
            TypedValue parameterValue;
            try {
                parameterValue = processor.Parse(member.GetParameterType(parameterIndex), instance, parameter);
            }
            catch (System.Exception e) {
                throw new ParseException<T>(member.Name, member.GetParameterType(parameterIndex), parameterIndex+1, parameter.Value, e);
            }
            return parameterValue.Value;
        }
    }
}
