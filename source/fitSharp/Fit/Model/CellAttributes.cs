// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;

namespace fitSharp.Fit.Model {
    public class CellAttributes {
	    public const string ActualKey = "actual";
	    public const string ExceptionKey = "exception";
	    public const string InformationPrefixKey = "informationPrefix";
	    public const string InformationSuffixKey = "informationSuffix";
	    public const string LabelKey = "label";
	    public const string AddKey = "add";
	    public const string ExtensionKey = "extension";

	    public const string StatusKey = "class";
	    public const string ExceptionStatus = "error";
	    public const string WrongStatus = "fail";
	    public const string IgnoreStatus = "ignore";
	    public const string RightStatus = "pass";

        public const string PrefixFormat = "{1} {0}";
        public const string SuffixFormat = "{0} {1}";

	    private Dictionary<string, string> attributes;

        public CellAttributes() {}

        public CellAttributes(CellAttributes other) {
            if (other.attributes != null) attributes = new Dictionary<string, string>(other.attributes);
        }

        public bool HasAttribute(string key) {
            return attributes == null ? false : attributes.ContainsKey(key);
        }

        public string GetAttribute(string key) {
            return HasAttribute(key) ? attributes[key] : string.Empty;
        }

        public void SetAttribute(string key, string value) {
            if (attributes == null) attributes = new Dictionary<string, string>();
            attributes[key] = value;
        }

        public void AddToAttribute(string key, string value, string format) {
            SetAttribute(key, string.Format(format, GetAttribute(key), value));
        }
    }
}
