using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iAnywhere.Data.SQLAnywhere;

namespace Simple.Data.SqlAnywhereTest
{
    internal static class DatabaseHelper
    {
        public static dynamic Open()
        {
            return Database.Opener.OpenConnection(Properties.Settings.Default.ConnectionString);
        }

        public static void Reset()
        {
            using (var cn = new SAConnection(Properties.Settings.Default.ConnectionString))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "TestReset";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
