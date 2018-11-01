using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sjd.FakeServer
{
    public static class FakeServerRegistrationExtensions
    {
        public static async Task<T> FirstOrDefaultAsync<T>(
            this IEnumerable<T> items,
            Func<T, Task<bool>> predicate)
        {
            foreach (T item in items)
            {
                if (await predicate(item))
                {
                    return item;
                }
            }

            return default(T);
        }
    }
}