using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using laget.Limiter.Limits;
using laget.Limiter.Stores;
using Xunit;

namespace laget.Limiter.Tests
{
    public class RateLimiterTests
    {
        readonly IStore _store;

        public RateLimiterTests()
        {
            _store = new MemoryStore();
        }

        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimit_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            limiter.Limit(() => { });

            Assert.Single((IEnumerable)limiter.History);
        }

        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimit_WithStaticCalls_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            limiter.Limit(3, () => { });

            Assert.Equal(3, limiter.History.Count());
        }

        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimit_WithDynamicCalls_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            limiter.Limit(() =>
            {
                limiter.Register(3);
            });

            Assert.Equal(4, limiter.History.Count());
        }

        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimit_WithStaticAndDynamicCalls_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            limiter.Limit(2, () =>
            {
                limiter.Register(3);
            });

            Assert.Equal(5, limiter.History.Count());
        }

        [Fact]
        public async Task RateLimiter_WhenOneCallMadeToLimitAsync_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            var limiter = config.BuildRateLimiter();
            await limiter.LimitAsync(() => Task.CompletedTask);

            Assert.Single((IEnumerable)limiter.History);
        }

        [Fact]
        public void RateLimiter_WhenOneCallMadeToLimitWithReturn_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            Func<int> returningMethod = () => 5;

            var limiter = config.BuildRateLimiter();
            var result = limiter.Limit(returningMethod);

            Assert.Single((IEnumerable)limiter.History);
        }

        [Fact]
        public async Task RateLimiter_WhenOneCallMadeToLimitAsyncWithReturn_AddsCallToCallTracker()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(5, TimeSpan.FromSeconds(1)));

            Func<Task<int>> returningMethod = async () => await Task.FromResult(5);

            var limiter = config.BuildRateLimiter();
            var result = await limiter.Limit(returningMethod);

            Assert.Single((IEnumerable)limiter.History);
        }

        [Fact]
        public async Task RateLimiter_WhenOneMoreCallThanLimitMadeInTimespan_LastCallTimeIsGreaterThanTimespanAfterFirstCall()
        {
            var callTimes = new List<DateTime>();

            Func<Task> callLogger = () =>
            {
                callTimes.Add(ReferenceTime.UtcNow);
                return Task.CompletedTask;
            };

            var rateLimitTimespan = TimeSpan.FromMilliseconds(200);

            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(3, rateLimitTimespan));

            var limiter = config.BuildRateLimiter();

            await Task.WhenAll(new List<Task>
            {
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger)
            });

            var firstCall = callTimes.Min();
            var lastCall = callTimes.Max();

            var elapsedTimeBetweenFirstAndLastCall = lastCall.Subtract(firstCall);

            Assert.True(elapsedTimeBetweenFirstAndLastCall.TotalMilliseconds > rateLimitTimespan.TotalMilliseconds, $"Time between first and last call should be at least {rateLimitTimespan.TotalMilliseconds} milliseconds but was '{elapsedTimeBetweenFirstAndLastCall.TotalMilliseconds}' milliseconds");
        }

        [Fact]
        public async Task RateLimiter_WhenOneMoreCallThanLimitMadeInTimespan_SaysReachedLimit()
        {
            var config = new Configuration();
            config.SetStore(_store);
            config.SetLimit(new StandardLimit(3, TimeSpan.FromMilliseconds(200)));

            var limiter = config.BuildRateLimiter();

            var timesLimitReached = 0;

            Func<Task> callLogger = () =>
            {
                if (limiter.IsExhausted)
                {
                    timesLimitReached++;
                }

                return Task.CompletedTask;
            };

            await Task.WhenAll(new List<Task>
            {
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger),
                limiter.LimitAsync(callLogger)
            });

            Assert.Equal(1, timesLimitReached);
        }
    }
}
