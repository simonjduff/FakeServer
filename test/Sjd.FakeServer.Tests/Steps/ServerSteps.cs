using System;
using System.Threading.Tasks;
using CheetahTesting;
using Sjd.FakeServer.Tests.ContextInterfaces;
using Xunit;

namespace Sjd.FakeServer.Tests.Steps
{
    public static class ServerSteps
    {
        public static void RegisterAUri<T>(this IGiven<T> given, string uri, string response)
            where T : IHasServer
        {
            given.Context.FakeServer.Register(new FakeServerRegistration
            {
                Uri = new Uri(uri),
                Response = response
            });
        }

        public static async Task MakeTheRequest<T>(this IWhen<T> when, string uri)
            where T : IHasServer, IHasResponse
        {
            var client = when.Context.FakeServer.GetClient();
            when.Context.ResponseMessage = await client.GetAsync(new Uri(uri));
        }

        public static async Task JsonIsReturned<T>(this IThen<T> then, string response)
            where T : IHasResponse
        {
            var content = await then.Context.ResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal(response, content);
        }
    }
}