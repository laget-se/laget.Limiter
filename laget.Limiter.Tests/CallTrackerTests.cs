using System;
using System.Linq;
using laget.Limiter.Limits;
using laget.Limiter.Stores;
using Xunit;

namespace laget.Limiter.Tests
{
    public class CallTrackerTests
    {
        readonly IStore _store;

        public CallTrackerTests()
        {
            _store = new MemoryStore();
        }

        [Fact]
        public void TrimCallsForRateLimits_ForNoRateLimits_KeepsAllCalls()
        {
            var callTracker = new Tracker(_store);
            callTracker.CallWillHappenIn(200);
            callTracker.TrimCallsForRateLimits();

            Assert.Single(callTracker.History);
        }

        [Fact]
        public void TrimCallsForRateLimits_ForOneRateLimit_TrimsToRateLimitAmount()
        {
            var callTracker = new Tracker(_store);
            callTracker.CallWillHappenIn(0);
            callTracker.CallWillHappenIn(100);
            callTracker.CallWillHappenIn(200);

            var rateLimit = new StandardLimit(2, TimeSpan.FromSeconds(1));

            callTracker.TrimCallsForRateLimits(rateLimit);

            Assert.Equal(2, callTracker.History.Count());
        }

        [Fact]
        public void TrimCallsForRateLimits_ForOneRateLimit_KeepsTheMostRecentCalls()
        {
            var now = new DateTime(2018, 01, 01);
            ReferenceTime.FreezeAtUtc(now);

            var callTracker = new Tracker(_store);
            callTracker.CallWillHappenIn(0);
            callTracker.CallWillHappenIn(1000);

            var rateLimit = new StandardLimit(1, TimeSpan.FromSeconds(1));

            callTracker.TrimCallsForRateLimits(rateLimit);

            Assert.Single(callTracker.History);
            Assert.Equal(now.AddMilliseconds(1000), callTracker.History.Single());

            ReferenceTime.Unfreeze();
        }
    }
}
