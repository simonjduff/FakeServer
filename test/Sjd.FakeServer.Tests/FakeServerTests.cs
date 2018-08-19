using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CheetahTesting;
using Sjd.FakeServer.Tests.ContextInterfaces;
using Sjd.FakeServer.Tests.Steps;
using Xunit;

namespace Sjd.FakeServer.Tests
{
    public class FakeServerTests
    {
        [Fact]
        public async Task SimpleUri()
        {
            string json = "{\"Test:\"Success\"}";
            
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri("http://fake.local/123", json))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .ExecuteAsync();
        }
        
        [Fact]
        public async Task TwoUrisRegistered()
        {
            string json = "{\"Test:\"Success\"}";
                
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri("http://fake.local/123", json))
                .And(i => i.RegisterAUri("http://fake.local/456", "{}"))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .ExecuteAsync();
        }

        private class FakeServerContext : IHasServer,
            IHasResponse
        {
            public FakeServer FakeServer { get; set; } = new FakeServer();
            public List<FakeServerRegistration> Registrations { get; set; } = new List<FakeServerRegistration>();
            public HttpResponseMessage ResponseMessage { get; set; }
        }
    }
}
