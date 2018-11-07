using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sjd.FakeServer
{
    public class FakeHttpHandler : DelegatingHandler
    {
        private readonly Guid _id;
        private readonly TimeSpan _timeout;

        public FakeHttpHandler(Guid id, TimeSpan timeout)
        {
            _id = id;
            _timeout = timeout;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
                var content = request?.Content != null
                    ? await request.Content?.ReadAsStringAsync()
                    : string.Empty;

                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var match = FakeServer.Registrations[_id]
                    .FirstOrDefault(reg =>
                        reg.Uri.Equals(request.RequestUri)
                        && reg.Method == request.Method
                        && reg.ContentMatchFunc(content));

                if (match == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                match.PreReturnAction();

                var message = new HttpResponseMessage(match.StatusCode);

                if (match.Response != null)
                {
                    message.Content = new StringContent(match.Response);
                }

                foreach (var header in match.Headers ?? new Dictionary<string, List<string>>())
                {
                    message.Headers.Add(header.Key, header.Value);
                }

                if (!string.IsNullOrWhiteSpace(match.ContentType))
                {
                    message.Content.Headers.ContentType.MediaType = match.ContentType;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                return message;
        }
    }
}