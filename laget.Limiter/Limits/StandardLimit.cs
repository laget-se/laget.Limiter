using System;
using System.Linq;

namespace laget.Limiter.Limits
{
    /// <summary>
    /// Limits the number of calls that can be made within a certain time period
    /// </summary>
    public class StandardLimit : Limit
    {
        public StandardLimit(int amountOfCalls, TimeSpan timeFrame)
            : base(amountOfCalls, timeFrame)
        {
        }

        public override int GetNextAllowedCallTime(Tracker callTracker)
        {
            var callHistory = callTracker.History
                .OrderByDescending(x => x)
                .Take(Amount)
                .ToList();

            if (!callHistory.Any() || callHistory.Count < Amount)
            {
                return 0;
            }

            var earliestCallTime = callHistory.OrderBy(x => x).First();
            var nextCallTime = earliestCallTime.Add(TimeSpan.FromMilliseconds(TimeFrame.TotalMilliseconds));
            var untilNextCall = nextCallTime.Subtract(ReferenceTime.UtcNow);

            var milliseconds = untilNextCall.TotalMilliseconds < 0 ? 0 : untilNextCall.TotalMilliseconds;

            return (int)Math.Ceiling(milliseconds);
        }
    }
}
