using System.Net.Http;

namespace Sjd.FakeServer.Tests.ContextInterfaces
{
    public interface IHasResponse
    {
        HttpResponseMessage ResponseMessage { get; set; }
    }
}