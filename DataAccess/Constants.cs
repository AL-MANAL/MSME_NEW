using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class Constants
    {
        public static string DbConnectionString => ConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ToString();
    }
}
