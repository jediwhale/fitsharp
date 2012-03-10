// Copyright © 2012 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace dbfit.util {
    public class ColumnAccessors {
		private readonly Dictionary<string, Accessor> accessors = new Dictionary<string, Accessor>();

        public void Assign(string key, Accessor accessor) {
            accessors[key] = accessor;
        }

        public RuntimeMember Find(MemberSpecification specification, Func<KeyValuePair<string, Accessor>, bool> filter) {
            foreach (KeyValuePair<string, Accessor> accessor in accessors) {
                if (!filter(accessor)) continue;
                if (specification.IsSetter) return new SetterMember(accessor.Value);
                if (specification.IsGetter) return new GetterMember(accessor.Value);
            }
            throw new ArgumentException(string.Format("Missing member '{0}'", specification));
        }

        private class SetterMember: RuntimeMember {
            private readonly Accessor accessor;

            public SetterMember(Accessor accessor) {
                this.accessor = accessor;
            }

            public TypedValue Invoke(object[] parameters) {
                accessor.Set(parameters[0]);
                return TypedValue.Void;
            }

            public bool MatchesParameterCount(int count) {
                return count == 1;
            }

            public Type GetParameterType(int index) {
                return accessor.DotNetType;
            }

            public string GetParameterName(int index) {
                return accessor.Name;
            }

            public Type ReturnType {
                get { return typeof(void); }
            }

            public string Name {
                get { return accessor.Name; }
            }
        }

        private class GetterMember: RuntimeMember {
            private readonly Accessor accessor;

            public GetterMember(Accessor accessor) {
                this.accessor = accessor;
            }

            public TypedValue Invoke(object[] parameters) {
                return new TypedValue(accessor.Get(), accessor.DotNetType);
            }

            public bool MatchesParameterCount(int count) {
                return count == 0;
            }

            public Type GetParameterType(int index) {
                return typeof(void);
            }

            public string GetParameterName(int index) {
                return accessor.Name;
            }

            public Type ReturnType {
                get { return accessor.DotNetType; }
            }

            public string Name {
                get { return accessor.Name; }
            }
        }
    }
}
