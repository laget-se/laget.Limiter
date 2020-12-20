using laget.Limiter.Limits;
using laget.Limiter.Stores;

namespace laget.Limiter
{
    public class Configuration
    {
        protected IStore Store;
        protected Limit RateLimit;

        public Configuration()
        {
        }

        public Configuration(IStore store, Limit limit)
        {
            Store = store;
            RateLimit = limit;
        }

        public void SetLimit<T>(T limit) where T : Limit
        {
            RateLimit = limit;
        }

        public void SetStore<T>(T store) where T : IStore
        {
            Store = store;
        }

        public ILimiter BuildRateLimiter()
        {
            return new Limiter(Store, RateLimit);
        }
    }
}
