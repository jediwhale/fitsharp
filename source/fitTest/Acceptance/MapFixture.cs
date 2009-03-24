using System.Collections;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class MapFixture: DoFixture {
        public SetFixture Map {
            get {
                Hashtable theMap = new Hashtable();
                theMap.Add("a", "b");     
                theMap.Add("c", "d");     
                return new SetFixture(theMap);
            }
        }
        public SubsetFixture SubsetMap {
            get {
                Hashtable theMap = new Hashtable();
                theMap.Add("a", "b");     
                theMap.Add("c", "d");     
                return new SubsetFixture(theMap);
            }
        }
    }
}