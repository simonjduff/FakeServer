using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Sjd.FakeServer
{
    public class FakeServer
    {
        private readonly Guid _id = Guid.NewGuid();

        internal static readonly ConcurrentDictionary<Guid, FakeServerRegistration[]> Registrations =
            new ConcurrentDictionary<Guid, FakeServerRegistration[]>(); 

        public void Register(Func<RegistrationBuilder,RegistrationBuilder> builderFunc)
        {
            var builder = new RegistrationBuilder();
#pragma warning disable CS0618 // Type or member is obsolete
            Register(builderFunc(builder).Build());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Obsolete("Use Register(Func<RegistrationBuilder,RegistrationBuilder>)")]
        public void Register(FakeServerRegistration registration)
        {
            Registrations.AddOrUpdate(_id,
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
            return new HttpClient(new FakeHttpHandler(_id));
        }
    }
}
