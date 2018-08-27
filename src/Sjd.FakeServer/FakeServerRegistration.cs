using System;
using System.Net.Http;

namespace Sjd.FakeServer
{
    public class FakeServerRegistration
    {
        public Uri Uri { get; set; }
        public string Response { get; set; }
        public HttpMethod Method { get; set; }
    }
}