using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Service;

namespace fitSharp.Test.Acceptance.Slim {
    public class SlimDomain {
        private readonly Service service = new Service();

        public string RoundTripForTypeValue(string typeName, string inputValue) {
            return service.Compose(service.Parse(Type.GetType(typeName), TypedValue.Void, new SlimLeaf(inputValue))).Value;
        }

        public string RoundTripForNullableInt(string inputValue) {
            string result = service.Compose(service.Parse(typeof(int?), TypedValue.Void, new SlimLeaf(inputValue ?? "null"))).Value;
            return result == "null" ? null : result;
        }

        public string LookupIn(string key, string dictionaryTable) {
            Dictionary<string, string> dictionary =
                service.Parse<string, Dictionary<string, string>>(dictionaryTable);
            return dictionary[key];
        }
    }
}
