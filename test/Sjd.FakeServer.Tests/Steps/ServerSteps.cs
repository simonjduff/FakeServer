using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CheetahTesting;
using Sjd.FakeServer.Tests.ContextInterfaces;
using Xunit;

namespace Sjd.FakeServer.Tests.Steps
{
    public static class ServerSteps
    {
        public static void RegisterAUri<T>(this IGiven<T> given, 
            Func<RegistrationBuilder, RegistrationBuilder> builder)
            where T : IHasServer
        {
            given.Context.FakeServer.Register(builder);
        }

        public static async Task MakeTheRequest<T>(this IWhen<T> when, 
            HttpClient client,
            string uri, 
            HttpMethod method = null,
            string body = null)
            where T : class, IHasServer, IHasResponse
        {

            var message = new HttpRequestMessage(method ?? HttpMethod.Get, new Uri(uri));

            if (!string.IsNullOrEmpty(body))
            {
                message.Content = new StringContent(body);
            }

            when.Context.ResponseMessage = await client.SendAsync(message);
        }

        public static async Task MakeTheRequest<T>(this IWhen<T> when,
            string uri,
            HttpMethod method = null,
            string body = null)
            where T : class, IHasServer, IHasResponse
        {
            var client = when.Context.FakeServer.GetClient();

            await MakeTheRequest<T>(when, client, uri, method, body);
        }

        public static async Task JsonIsReturned<T>(this IThen<T> then, string response)
            where T : IHasResponse
        {
            Assert.NotEqual(HttpStatusCode.NotFound, then.Context.ResponseMessage.StatusCode);

            if (response == null)
            {
                Assert.Null(then.Context.ResponseMessage.Content);
                return;
            }

            var content = await then.Context.ResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal(response, content);
        }

        public static void ResponseHeader<T>(this IThen<T> then, string header, string value)
        where T : IHasResponse
        {
            Assert.True(then.Context.ResponseMessage.Headers.Contains(header));
            Assert.Contains(value, then.Context.ResponseMessage.Headers.GetValues(header));
        }

        public static void ContentTypeIs<T>(this IThen<T> then, string contentType)
        where T : IHasResponse
        {
            Assert.Equal(contentType, then.Context.ResponseMessage.Content.Headers.ContentType.MediaType);
        }

        public static void StatusCodeIs<T>(this IThen<T> then, HttpStatusCode statusCode)
            where T : IHasResponse
        {
            Assert.Equal(statusCode, then.Context.ResponseMessage.StatusCode);
        }
    }
}