using Spinoza.Backend.Managers.TestCatalog.Tests.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{
    public interface ISignalRWrapper
    {
        Task StartSignalR();
        Task<bool> WaitForSignalREventAsync(int timeoutInSeconds = 10);
        IList<TestChangeResult?> Messages { get; }
    }
}
