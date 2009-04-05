// FitNesse.NET
// Copyright © 2006-2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Web;
using System.Text;

namespace fit.Operators {
    public interface CellFactory {
        bool CanMake(object theValue);
        Parse Make(object theValue, string theFormat);
    }

    public class CellFactoryRepository {
        public const string Grey = "<span class=\"fit_grey\">{0}</span>";

        public static CellFactoryRepository Instance {get {return ourInstance;}}
        private static CellFactoryRepository ourInstance = new CellFactoryRepository();

        private CellFactoryRepository() {
            myFactories = new ArrayList();
            myFactories.Add(new TableAdapter());
            myFactories.Add(new DefaultFactory());
        }

        public Parse Make(object theValue) { return Make(theValue, "{0}"); }

        public Parse Make(object theValue, string theFormat) {
            foreach (CellFactory factory in myFactories) {
                if (factory.CanMake(theValue)) return factory.Make(theValue, theFormat);
            }
            throw new IndexOutOfRangeException("no factories");
        }

        private ArrayList myFactories;

        private class DefaultFactory: CellFactory { //todo: use to replace all cell construction
            
            public bool CanMake(object theValue) {return true;}

            public Parse Make(object theValue, string theFormat) {
                string valueString = string.Empty;
                if (theValue != null) {
                    if (theValue is Array) {
                        StringBuilder result = new StringBuilder();
                        foreach (object value in (IEnumerable)theValue) {
                            if (result.Length > 0) result.Append(",");
                            result.Append(value.ToString());
                        }
                        valueString =  result.ToString();
                    }
                    else
                        valueString = theValue.ToString();
                }
                return new Parse("td", string.Format(theFormat, HttpUtility.HtmlEncode(valueString)), null, null);
            }
        }
    }
}