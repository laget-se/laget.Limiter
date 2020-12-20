using System;

namespace laget.Limiter.Limits
{
    public abstract class Limit
    {
        public int Amount { get; }

        public TimeSpan TimeFrame { get; }

        protected Limit(int amount, TimeSpan timeFrame)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("The amount in a rate limit must be a positive value");
            }

            Amount = amount;
            TimeFrame = timeFrame;
        }

        /// <summary>
        /// Gets the next allowed call time in milliseconds
        /// </summary>
        /// <returns></returns>
        public abstract int GetNextAllowedCallTime(Tracker callTracker);
    }
}
