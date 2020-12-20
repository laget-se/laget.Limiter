using System;
using System.Collections.Generic;

namespace laget.Limiter.Stores
{
    public interface IStore
    {
        void Add(DateTime item);
        IList<DateTime> Get();

        void Trim(int amount);
    }
}
