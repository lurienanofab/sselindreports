using LNF.Cache;
using LNF.CommonTools;
using LNF.Repository;
using System;
using System.Data;

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

        [Obsolete]
        public static UnitOfWorkAdapter AddParameter(this UnitOfWorkAdapter adap, string name, object value)
        {
            var p = adap.SelectCommand.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            adap.SelectCommand.Parameters.Add(p);
            return adap;
        }

        [Obsolete]
        public static UnitOfWorkAdapter AddParameterIf(this UnitOfWorkAdapter adap, string name, bool condition, object value)
        {
            if (condition)
                AddParameter(adap, name, value);
            return adap;
        }

        [Obsolete]
        public static UnitOfWorkAdapter CommandTypeText(this UnitOfWorkAdapter adap)
        {
            adap.SelectCommand.CommandType = CommandType.Text;
            return adap;
        }

        [Obsolete]
        public static UnitOfWorkAdapter ApplyParameters(this UnitOfWorkAdapter adap, object queryParams)
        {
            var dict = Utility.ObjectToDictionary(queryParams);

            foreach(var kvp in dict)
            {
                AddParameter(adap, kvp.Key, kvp.Value);
            }

            return adap;
        }
    }
}
