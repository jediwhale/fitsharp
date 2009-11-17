using System;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Service;

namespace fitSharp.Test.Acceptance.Slim {
    public class SlimDomain {
        private readonly Service service = new Service();
        public string RoundTripConvertType(string inputValue, string typeName) {
            return service.Compose(service.Parse(Type.GetType(typeName), TypedValue.Void, new SlimLeaf(inputValue))).Value;
        }
    }
}
