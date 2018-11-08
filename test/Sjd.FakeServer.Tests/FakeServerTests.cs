using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CheetahTesting;
using Sjd.FakeServer.Tests.ContextInterfaces;
using Sjd.FakeServer.Tests.Steps;
using Xunit;
using Xunit.Abstractions;

namespace Sjd.FakeServer.Tests
{
    public class FakeServerTests
    {
        private readonly ITestOutputHelper _output;

        public FakeServerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SimpleUri()
        {
            string json = "{\"Test:\"Success\"}";
            
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .ExecuteAsync();
        }

        [Fact]
        public async Task ResponseHeaders()
        {
            string json = "{\"Test:\"Success\"}";

            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithResponseHeader("Header1", "Value1")
                    .WithResponseHeader("Header1", "Value2")))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .And(t => t.ResponseHeader("Header1", "Value1"))
                .And(t => t.ResponseHeader("Header1", "Value2"))
                .ExecuteAsync();
        }

        [Fact]
        public async Task ContentTypeHeader()
        {
            string json = "{\"Test:\"Success\"}";

            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithContentType("application/vnd.custom")))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .And(t => t.ContentTypeIs("application/vnd.custom"))
                .ExecuteAsync();
        }

        [Fact]
        public async Task StatusCode()
        {
            string json = "{\"Test:\"Success\"}";

            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithStatusCode(HttpStatusCode.Accepted)))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .And(t => t.StatusCodeIs(HttpStatusCode.Accepted))
                .ExecuteAsync();
        }

        [Fact]
        public async Task NullBody()
        {
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(null)
                    .WithStatusCode(HttpStatusCode.Accepted)))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(null))
                .And(t => t.StatusCodeIs(HttpStatusCode.Accepted))
                .ExecuteAsync();
        }

        [Fact]
        public async Task TwoUrisRegistered()
        {
            string json = "{\"Test:\"Success\"}";
                
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)))
                .And(i => i.RegisterAUri(b => b.WithUri("http://fake.local/456")
                    .WithResponse("{}")))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123"))
                .ThenAsync(t => t.JsonIsReturned(json))
                .ExecuteAsync();
        }

        [Fact]
        public async Task PostUriRegistered()
        {
            string json = "{\"Test:\"Success\"}";
                
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse("{}")))
                .And(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithMethod(HttpMethod.Post)))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123", HttpMethod.Post))
                .ThenAsync(t => t.JsonIsReturned(json))
                .ExecuteAsync();
        }

        [Fact]
        public async Task LambdaMatcher()
        {
            string json = "{\"Test:\"Success\"}";
            string body = "{\"MatchMe:\"123\"}";

            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse("{}")))
                .And(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithMethod(HttpMethod.Post)
                    .WithContentMatch(content =>
                        content?.Equals(body) ?? false)
                    ))
                .WhenAsync(i => i.MakeTheRequest("http://fake.local/123", HttpMethod.Post, body))
                .ThenAsync(t => t.JsonIsReturned(json))
                .ExecuteAsync();
        }

        [Fact]
        public async Task PrereturnLambda()
        {
            string json = "{\"Test:\"Success\"}";

            var stopwatch = new Stopwatch();

            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithMethod(HttpMethod.Get)
                    .WithBeforeReturn(() => Thread.Sleep(1000))
                ))
                .WhenAsync(async i =>
                {
                    stopwatch.Start();
                    await i.MakeTheRequest("http://fake.local/123", HttpMethod.Get);
                    stopwatch.Stop();
                })
                .ThenAsync(t => t.JsonIsReturned(json))
                .And(c => Assert.True(stopwatch.ElapsedMilliseconds > 1000))
                .ExecuteAsync();
        }

        [Fact]
        public async Task Timeout()
        {
            string json = "{\"Test:\"Success\"}";

            var stopwatch = new Stopwatch();
var sw1 = new Stopwatch();
sw1.Start();
            await CTest<FakeServerContext>
                .Given(i => i.RegisterAUri(b => b.WithUri("http://fake.local/123")
                    .WithResponse(json)
                    .WithMethod(HttpMethod.Get)
                    .WithBeforeReturn(t => Task.Delay(5000, t))
                ))
                .WhenAsync(async i =>
                {
                    var client = i.Context.FakeServer.GetClient();
                    client.Timeout = TimeSpan.FromMilliseconds(100);
                    try
                    {
                        stopwatch.Start();
                        await i.MakeTheRequest(client, "http://fake.local/123", HttpMethod.Get);
                        stopwatch.Stop();
                        Assert.True(stopwatch.ElapsedMilliseconds < 500);
                    }
                    catch (Exception e)
                    {
                        i.Context.Exception = e;
                    }
                })
                .Then(t => Assert.True(t.Context.Exception is TaskCanceledException))
                .ExecuteAsync();
        }

        private class FakeServerContext : IHasServer,
            IHasResponse,
            IHasException
        {
            public FakeServer FakeServer { get; set; } = new FakeServer();
            public List<FakeServerRegistration> Registrations { get; set; } = new List<FakeServerRegistration>();
            public HttpResponseMessage ResponseMessage { get; set; }
            public Exception Exception { get; set; }
        }

        public interface IHasException
        {
            Exception Exception { get; set; }
        }
    }
}
