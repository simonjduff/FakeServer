using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Sjd.FakeServer
{
    public class FakeServer
    {
        private readonly Guid Id = Guid.NewGuid();

        internal static readonly ConcurrentDictionary<Guid, FakeServerRegistration[]> Registrations =
            new ConcurrentDictionary<Guid, FakeServerRegistration[]>(); 

        public void Register(FakeServerRegistration registration)
        {
            Registrations.AddOrUpdate(Id,
                new [] {registration},
                (id, cur) =>
                {
                    FakeServerRegistration[] newArray = new FakeServerRegistration[cur.Length + 1];
                    cur.CopyTo(newArray, 0);
                    newArray[newArray.Length - 1] = registration;
                    return newArray;
                });
        }

        public HttpClient GetClient()
        {
            return new HttpClient(new FakeHttpHandler(Id));
        }
    }
}
