using System.Configuration;

namespace DataAccess
{
    public static class Constants
    {
        public static string DbConnectionString => ConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ToString();
    }
}