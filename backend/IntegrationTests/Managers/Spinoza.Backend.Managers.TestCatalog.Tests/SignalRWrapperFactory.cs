using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{
    public class SignalRWrapperFactory : ISignalRWrapperFactory
    {
        ISignalRWrapper ISignalRWrapperFactory.Create(ITestOutputHelper testOutputHelper)
        {
            return new SignalRWrapper(testOutputHelper);
        }
    }
}
