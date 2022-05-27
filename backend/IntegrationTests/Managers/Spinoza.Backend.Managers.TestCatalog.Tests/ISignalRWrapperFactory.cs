using Xunit.Abstractions;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{
    public interface ISignalRWrapperFactory
    {
        ISignalRWrapper Create(ITestOutputHelper testOutputHelper);
    }
}
