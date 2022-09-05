using System;
using System.Linq;

namespace laget.Limiter.Extensions
{
    public static class LimiterExtension
    {
        public static void Warning(this ILimiter limiter, Action action, double cutoff = 0.85)
        {
            if (limiter.History.Count() >= (Math.Ceiling(limiter.RateLimit.Amount * cutoff)))
            {
                action.Invoke();
            }
        }

        public static void Exhausted(this ILimiter limiter, Action action)
        {
            if (limiter.IsExhausted)
            {
                action.Invoke();
            }
        }
    }
}
