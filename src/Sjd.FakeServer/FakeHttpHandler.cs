using System;
using System.Collections.Generic;
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
                .FirstOrDefault(reg => 
                    reg.Uri.Equals(request.RequestUri)
                    && reg.Method == request.Method
                    );
            var message = new HttpResponseMessage
            {
                Content = new StringContent(match.Response),
            };

            foreach (var header in match.Headers ?? new Dictionary<string, List<string>>())
            {
                message.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrWhiteSpace(match.ContentType))
            {
                message.Content.Headers.ContentType.MediaType = match.ContentType;
            }

            return Task.FromResult(message);
        }
    }
}