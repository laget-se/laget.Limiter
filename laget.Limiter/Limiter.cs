using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using laget.Limiter.Limits;
using laget.Limiter.Stores;

namespace laget.Limiter
{
    public interface ILimiter
    {
        IEnumerable<DateTime> History { get; }

        Limit RateLimit { get; }

        bool IsExhausted { get; }

        int ResetsAt { get; }

        void Register(int calls);

        void Limit(Action limitedCall);

        void Limit(int calls, Action limitedCall);

        T Limit<T>(Func<T> limitedCall);

        Task LimitAsync(Func<Task> limitedCall);

        Task<T> LimitAsync<T>(Func<Task<T>> limitedCall);
    }

    public class Limiter : ILimiter
    {
        private static readonly object Lock;

        public Tracker Tracker { get; }

        public Limit RateLimit { get; }

        public IEnumerable<DateTime> History => Tracker.History;

        public int ResetsAt => RateLimit.GetNextAllowedCallTime(Tracker);

        public bool IsExhausted
        {
            get
            {
                lock (Lock)
                {
                    var nextCallTime = RateLimit.GetNextAllowedCallTime(Tracker);

                    return nextCallTime > 0;
                }
            }
        }

        public Limiter(IStore store, Limit rateLimit)
        {
            RateLimit = rateLimit;
            Tracker = new Tracker(store);
        }

        static Limiter()
        {
            Lock = new object();
        }

        public void Register(int calls)
        {
            var nextCallTime = GetNextCallTime(calls);

            Task.Delay(nextCallTime).Wait();
        }

        public void Limit(Action limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            Task.Delay(nextCallTime).Wait();
            limitedCall();
        }

        public void Limit(int calls, Action limitedCall)
        {
            var nextCallTime = GetNextCallTime(calls);

            Task.Delay(nextCallTime).Wait();
            limitedCall();
        }

        public T Limit<T>(Func<T> limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            Task.Delay(nextCallTime).Wait();
            return limitedCall();
        }

        public async Task LimitAsync(Func<Task> limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            await Task.Delay(nextCallTime);
            await limitedCall();
        }

        public async Task<T> LimitAsync<T>(Func<Task<T>> limitedCall)
        {
            var nextCallTime = GetNextCallTime();

            await Task.Delay(nextCallTime);
            return await limitedCall();
        }

        private int GetNextCallTime(int calls = 1)
        {
            int nextCallTime;

            lock (Lock)
            {
                nextCallTime = RateLimit.GetNextAllowedCallTime(Tracker);

                for (var i = 0; i < calls; i++)
                {
                    Tracker.CallWillHappenIn(nextCallTime);
                }

                Tracker.TrimCallsForRateLimits(RateLimit);
            }

            return nextCallTime;
        }
    }
}
