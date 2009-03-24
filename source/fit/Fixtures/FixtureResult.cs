// FitNesse.NET
// Copyright © 2006,2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Xml;
using fitlibrary;

namespace fit {

	public class FixtureResult {

        public static object Wrap(object theResult) {
            if (theResult == null) return null;
            Type resultType = theResult.GetType();
            if (resultType.IsPrimitive) return theResult;
            if (theResult is string) return theResult;
            if (theResult is Fixture) return theResult;
            if (IsObjectArray(resultType)) return new ArrayFixture((object[])theResult);
            if (typeof(IDictionary).IsAssignableFrom(resultType)) return new SetFixture(((IDictionary)theResult).Values);
            if (typeof(ICollection).IsAssignableFrom(resultType)) return new ArrayFixture((ICollection)theResult);
            if (typeof(IEnumerator).IsAssignableFrom(resultType)) return new ArrayFixture((IEnumerator)theResult);
            if (typeof(DataTable).IsAssignableFrom(resultType)) return new ArrayFixture((DataTable)theResult);
            if (typeof(XmlDocument).IsAssignableFrom(resultType)) return new XmlFixture((XmlDocument)theResult);
            if (typeof(IEnumerable).IsAssignableFrom(resultType)) return new ArrayFixture(((IEnumerable)theResult).GetEnumerator());
            if (HasStaticParseMethod(resultType)) return theResult;
            return new DoFixture(theResult);
        }

        private static bool HasStaticParseMethod(Type theResultType) {
            MethodInfo parseMethod = theResultType.GetMethod(
                    "Parse",
                    BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Public,
                    null, new Type[] {typeof (string)}, null);
            return (parseMethod != null && parseMethod.ReturnType == theResultType);
        }

        private static bool IsObjectArray(Type theResultType) {
            return (theResultType.IsArray
                    && !theResultType.GetElementType().IsArray
                    && theResultType.GetElementType().IsAssignableFrom(typeof(object)));
        }
    }
}
