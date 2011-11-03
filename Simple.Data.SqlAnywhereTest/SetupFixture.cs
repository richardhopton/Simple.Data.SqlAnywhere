using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using iAnywhere.Data.SQLAnywhere;

namespace Simple.Data.SqlAnywhereTest
{
    [SetUpFixture]
    public class SetupFixture
    {
        [SetUp]
        public void CreateStoredProcedures()
        {
            using (var cn = new SAConnection(Properties.Settings.Default.ConnectionString))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    var script = Regex.Split(Properties.Resources.DatabaseReset, @"^\s*;\s*$", RegexOptions.Multiline)
                                      .Select(s=>s.Trim())
                                      .Where(s=> !String.IsNullOrWhiteSpace(s));
                    foreach (var sql in script)
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
