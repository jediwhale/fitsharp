using fitlibrary;
using fit.Engine;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit {
    public class ConfigurationSetupFixture: DoFixture {
        public ConfigurationSetupFixture(): base(Context.Configuration) {}

        public DoFixture Settings() {
            return new DoFixture(Context.Configuration.GetItem<Settings>());
        }

        public DoFixture ApplicationUnderTest() { return new DoFixture(Context.Configuration.GetItem<ApplicationUnderTest>()); }

        public DoFixture Service() { return new DoFixture(Context.Configuration.GetItem<Service>()); }

        public DoFixture GetItem(string type) { return new DoFixture(Context.Configuration.GetItem(type));}
    }
}
