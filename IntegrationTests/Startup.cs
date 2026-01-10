using Xunit.Abstractions;
using Xunit.Sdk;
[assembly: Xunit.TestFramework("IntegrationTests.Startup", "IntegrationTests")]

namespace IntegrationTests
{
    public class Startup : XunitTestFramework
    {
        public Startup(IMessageSink messageSink)
            : base(messageSink)
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
        }
    }
}
