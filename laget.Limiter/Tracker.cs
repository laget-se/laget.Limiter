using System;
using System.Collections.Generic;
using laget.Limiter.Limits;
using laget.Limiter.Stores;

namespace laget.Limiter
{
    public class Tracker
    {
        private readonly IStore _store;

        protected IEnumerable<DateTime> _history => _store.Get();

        public IEnumerable<DateTime> History => _history;

        public Tracker()
        {
        }

        public Tracker(IStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Records when the next call is scheduled to occur
        /// </summary>
        /// <param name="milliseconds"></param>
        public void CallWillHappenIn(double milliseconds)
        {
            _store.Add(ReferenceTime.UtcNow.AddMilliseconds(milliseconds));
        }

        public void TrimCallsForRateLimits()
        {
        }

        /// <summary>
        /// Will trim down the number of calls in the tracker to the amount required by these rate limits
        /// </summary>
        /// <param name="rateLimit"></param>
        public void TrimCallsForRateLimits(Limit rateLimit)
        {
            if (rateLimit == null)
            {
                return;
            }

            TrimToMostRecentCalls(rateLimit.Amount);
        }

        private void TrimToMostRecentCalls(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount of calls must be zero or a positive value");
            }

            _store.Trim(amount);
        }
    }
}
