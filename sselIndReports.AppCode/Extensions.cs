using LNF.Cache;

namespace sselIndReports.AppCode
{
    public static class CacheManagerExtensions
    {
        public static bool Updated(this CacheManager cacheManager)
        {
            return cacheManager.GetSessionValue("Updated", () => false);
        }

        public static void Updated(this CacheManager cacheManager, bool value)
        {
            cacheManager.SetSessionValue("Updated", value);
        }
    }
}
