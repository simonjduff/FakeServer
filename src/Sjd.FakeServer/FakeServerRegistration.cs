using System;
using System.Net.Http;

namespace Sjd.FakeServer
{
    public class FakeServerRegistration
    {
        public Uri Uri { get; set; }
        public string Response { get; set; }
        private HttpMethod _httpMethod = null;

        public HttpMethod Method
        {
            get => _httpMethod ?? HttpMethod.Get;
            set => _httpMethod = value;
        }
    }
}