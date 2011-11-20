using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;
using iAnywhere.Data.SQLAnywhere;

namespace Simple.Data.SqlAnywhere
{
    [Export(typeof(IConnectionProvider))]
    [Export("iAnywhere.Data.SqlAnywhere", typeof(IConnectionProvider))]
    public class SqlAnywhereConnectionProvider : IConnectionProvider, IServiceProvider
    {
        private string _connectionString;
        private SqlAnywhereQueryPager _queryPager;

        public SqlAnywhereConnectionProvider()
        {
            
        }

        internal Boolean GetSupportsCommonTableExpressions()
        {
            try
            {
                using (var connection = this.CreateConnection() as SAConnection)
                {
                    connection.Open();
                    var version = new Version(connection.ServerVersion);
                    return version.Major > 8;
                }
            }
            catch
            { }
            return false;
        }

        public SqlAnywhereConnectionProvider(string connectionString)
        {
            if (_connectionString != connectionString)
            {
                _connectionString = connectionString;
                _queryPager = null;
            }
        }

        public IDbConnection CreateConnection()
        {
            return new SAConnection(_connectionString);
        }

        public ISchemaProvider GetSchemaProvider()
        {
            return new SqlAnywhereSchemaProvider(this);
        }

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public string GetIdentityFunction()
        {
            return "@@IDENTITY";
        }

        public bool TryGetNewRowSelect(Table table, ref string insertSql, out string selectSql)
        {
            var identityColumn = table.Columns.FirstOrDefault(col => col.IsIdentity);

            if (identityColumn == null)
            {
                selectSql = null;
                return false;
            }

            selectSql = "select * from " + table.QualifiedName + " where " + identityColumn.QuotedName +
                        " = @@IDENTITY";
            return true;
        }

        public bool SupportsCompoundStatements
        {
            get { return false; }
        }

        public bool SupportsStoredProcedures
        {
            get { return true; }
        }

        public IProcedureExecutor GetProcedureExecutor(AdoAdapter adapter, ObjectName procedureName)
        {
            return new ProcedureExecutor(adapter, procedureName);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IQueryPager))
            {
                return _queryPager ?? (_queryPager = new SqlAnywhereQueryPager(this.GetSupportsCommonTableExpressions()));
            }
            return null;
        }
    }
}
