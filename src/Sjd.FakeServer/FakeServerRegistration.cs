using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sjd.FakeServer
{
    public class FakeServerRegistration
    {
        public Uri Uri { get; set; }
        public string Response { get; set; }
        private HttpMethod _httpMethod = null;
        public Dictionary<string, List<string>> Headers { get; set; }


        public HttpMethod Method
        {
            get => _httpMethod ?? HttpMethod.Get;
            set => _httpMethod = value;
        }

        public string ContentType { get; set; }
        public HttpContent Body { get; set; }
        public Func<string, bool> ContentMatchFunc { get; set; }
        public Func<CancellationToken, Task> PreReturnAction { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class RegistrationBuilder
    {
        private Uri _uri;
        private HttpMethod _httpMethod;
        private readonly Dictionary<string,List<string>> _responseHeaders = new Dictionary<string, List<string>>();
        private string _response;
        private string _contentType;
        private Func<string, bool> _matchFunc = m => true;
        private Func<CancellationToken,Task> _preReturnAction = t => Task.CompletedTask;
        private HttpStatusCode _statusCode = HttpStatusCode.OK;

        public RegistrationBuilder WithUri(Uri uri)
        {
            _uri = uri;
            return this;
        }

        public RegistrationBuilder WithUri(string uri)
        {
            _uri = new Uri(uri);
            return this;
        }

        public RegistrationBuilder WithMethod(HttpMethod method)
        {
            _httpMethod = method;
            return this;
        }

        public RegistrationBuilder WithResponse(string response)
        {
            _response = response;
            return this;
        }

        public RegistrationBuilder WithResponseHeader(string header, string value)
        {
            if (!_responseHeaders.ContainsKey(header))
            {
                _responseHeaders.Add(header, new List<string>());
            }

            _responseHeaders[header].Add(value);

            return this;
        }

        public RegistrationBuilder WithContentType(string contentType)
        {
            _contentType = contentType;
            return this;
        }

        [Obsolete("Use WithResponse")]
        public RegistrationBuilder WithBody(string body)
        {
            return WithResponse(body);
        }

        public RegistrationBuilder WithContentMatch(Func<string, bool> matchFunc)
        {
            _matchFunc = matchFunc;
            return this;
        }

        public RegistrationBuilder WithBeforeReturn(Func<CancellationToken, Task> action)
        {
            _preReturnAction = action;
            return this;
        }

        public RegistrationBuilder WithBeforeReturn(Action action)
        {
            _preReturnAction = t => {
                action.Invoke();
                return Task.CompletedTask;
            };
            return this;
        }

        public RegistrationBuilder WithStatusCode(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            return this;
        }

        public FakeServerRegistration Build()
        {
            return new FakeServerRegistration
            {
                Uri = _uri,
                Method = _httpMethod,
                Response = _response,
                Headers = _responseHeaders,
                ContentType = _contentType,
                ContentMatchFunc = _matchFunc,
                PreReturnAction = _preReturnAction,
                StatusCode = _statusCode
            };
        }
    }
}