using System;
using System.Collections.Generic;
using System.Net.Http;

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
    }

    public class RegistrationBuilder
    {
        public static RegistrationBuilder Register()
        {
            return new RegistrationBuilder();
        }

        private Uri _uri;
        private HttpMethod _httpMethod;
        private readonly Dictionary<string,List<string>> _responseHeaders = new Dictionary<string, List<string>>();
        private string _response;
        private string _contentType;

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

        public FakeServerRegistration Build()
        {
            return new FakeServerRegistration
            {
                Uri = _uri,
                Method = _httpMethod,
                Response = _response,
                Headers = _responseHeaders,
                ContentType = _contentType
            };
        }
    }
}