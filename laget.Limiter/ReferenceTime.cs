using System;

namespace laget.Limiter
{
    public static class ReferenceTime
    {
        static Func<DateTime> _utcDateTime;

        static ReferenceTime()
        {
            _utcDateTime = () => DateTime.UtcNow;
        }

        public static DateTime UtcNow => _utcDateTime();

        public static void FreezeAtUtc(DateTime utcDateTime)
        {
            _utcDateTime = () => utcDateTime;
        }

        public static void Unfreeze()
        {
            _utcDateTime = () => DateTime.UtcNow;
        }
    }
}
