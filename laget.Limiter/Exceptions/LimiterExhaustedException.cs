using System;
using System.Linq;
using laget.Limiter.Limits;

namespace laget.Limiter.Exceptions
{
    public class LimiterExhaustedException : Exception
    {
        public LimiterExhaustedException()
        {
        }

        public LimiterExhaustedException(string message)
            : base(message)
        {
        }

        public LimiterExhaustedException(ILimiter limiter)
            : base($"We've ran out of requests {limiter.History.Count()} out of {limiter.RateLimit.Amount} over {limiter.RateLimit.TimeFrame} will resets at {DateTime.Now.AddMilliseconds(limiter.ResetsAt)}")
        {
        }
    }
}
