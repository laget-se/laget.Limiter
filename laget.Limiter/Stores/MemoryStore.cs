﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace laget.Limiter.Stores
{
    public class MemoryStore : IStore
    {
        private List<DateTime> _store;

        public MemoryStore()
        {
            _store = new List<DateTime>();
        }

        public void Add(DateTime item)
        {
            _store.Add(item);
        }

        public IList<DateTime> Get()
        {
            return _store;
        }

        public void Trim(int amount)
        {
            _store = _store
                .OrderByDescending(x => x)
                .Take(amount)
                .ToList();
        }
    }
}
