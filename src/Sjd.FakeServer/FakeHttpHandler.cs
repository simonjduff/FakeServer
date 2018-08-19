using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sjd.FakeServer
{
    public class FakeHttpHandler : DelegatingHandler
    {
        private readonly Guid _id;

        public FakeHttpHandler(Guid id)
        {
            _id = id;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var match = FakeServer.Registrations[_id]
                .FirstOrDefault(reg => reg.Uri.Equals(request.RequestUri));
            var message = new HttpResponseMessage
            {
                Content = new StringContent(match.Response)
            };
            return Task.FromResult(message);
        }
    }
}