using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using iAnywhere.Data.SQLAnywhere;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.SqlAnywhere
{
    class SqlAnywhereSchemaProvider : ISchemaProvider
    {
        private readonly IConnectionProvider _connectionProvider;

        public SqlAnywhereSchemaProvider(IConnectionProvider connectionProvider)
        {
            if (connectionProvider == null) throw new ArgumentNullException("connectionProvider");
            _connectionProvider = connectionProvider;
        }

        public IConnectionProvider ConnectionProvider
        {
            get { return _connectionProvider; }
        }

        public IEnumerable<Table> GetTables()
        {
            return GetSchema("Tables").Select(SchemaRowToTable);
        }

        private static Table SchemaRowToTable(DataRow row)
        {
            return new Table(row["TABLE_NAME"].ToString(), row["TABLE_SCHEMA"].ToString(),
                        row["TABLE_TYPE"].ToString() == "BASE TABLE" ? TableType.Table : TableType.View);
        }

        public IEnumerable<Column> GetColumns(Table table)
        {
            if (table == null) throw new ArgumentNullException("table");
            var cols = GetColumnsDataTable(table);
            return cols.AsEnumerable().Select(row => SchemaRowToColumn(table, row));
        }

        private static Column SchemaRowToColumn(Table table, DataRow row)
        {
            return new SqlAnywhereColumn(row["name"] as String, table, Convert.ToBoolean(row["is_identity"]),
                              DbTypeFromDbTypeName(row["type_name"] as String) ??
                              DbTypeFromDbTypeName(row["domain_name"] as String) ?? SADbType.Char,
                              Convert.ToInt16(row["max_length"]));
        }

        public IEnumerable<Procedure> GetStoredProcedures()
        {
            return GetSchema("Procedures").Select(SchemaRowToStoredProcedure);
        }

        private IEnumerable<DataRow> GetSchema(string collectionName, params string[] constraints)
        {
            using (var cn = ConnectionProvider.CreateConnection())
            {
                cn.Open();

                return cn.GetSchema(collectionName, constraints).AsEnumerable();
            }
        }

        private static Procedure SchemaRowToStoredProcedure(DataRow row)
        {
            return new Procedure(row["PROCEDURE_NAME"].ToString(), row["PROCEDURE_NAME"].ToString(), row["PROCEDURE_SCHEMA"].ToString());
        }

        public IEnumerable<Parameter> GetParameters(Procedure storedProcedure)
        {
            var paramsSql =
                string.Format(
                    "select pp.parm_name, pp.parm_id, pp.parm_type, pp.parm_mode_in, pp.parm_mode_out, dom.domain_name, " +
                    "(select type_name from sysusertype where type_id=pp.user_type) as type_name, pp.width, pp.scale " +
                    "from sys.sysprocparm as pp " +
                    "key join sys.sysprocedure as p " +
                    "key join sys.sysuserperm as up " +
                    "key join sys.sysdomain as dom " +
                    "where ( ( pp.parm_type = 0 ) or ( pp.parm_type = 4 ) ) and " +
                    "up.user_name='{0}' and p.proc_name='{1}' " +
                    "order by up.user_name, p.proc_name, pp.parm_id",
                    storedProcedure.Schema, storedProcedure.Name);

            return SelectToDataTable(paramsSql).AsEnumerable()
                                               .Select(row => ParamsRowToSAParameter(row))
                                               .Select(p => new Parameter(p.ParameterName,
                                                                          SqlAnywhereTypeResolver.GetClrType(p.SADbType),
                                                                          p.Direction, p.DbType, p.Size));
        }

        private static SAParameter ParamsRowToSAParameter(DataRow row)
        {
            var parameter = new SAParameter();
            parameter.ParameterName = row["parm_name"] as String;
            parameter.Size = Convert.ToInt32(row["width"]);
            parameter.Scale = Convert.ToByte(row["scale"]);
            if (Convert.ToByte(row["parm_type"]) == 0)
            {
                var isInput = String.Equals(row["parm_mode_in"] as String, "Y", StringComparison.OrdinalIgnoreCase);
                var isOutput = String.Equals(row["parm_mode_out"] as String, "Y", StringComparison.OrdinalIgnoreCase);
                if (isInput)
                {
                    if (isOutput)
                    {
                        parameter.Direction = ParameterDirection.InputOutput;
                    }
                    else
                    {
                        parameter.Direction = ParameterDirection.Input;
                    }
                }
                else if (isOutput)
                {
                    parameter.Direction = ParameterDirection.Output;
                }
            }
            else
            {
                parameter.Direction = ParameterDirection.ReturnValue;
            }
            parameter.SADbType = DbTypeFromDbTypeName(row["domain_name"] as String) ??
                                 DbTypeFromDbTypeName(row["type_name"] as String) ?? SADbType.Char;
            switch (parameter.SADbType)
            {
                case SADbType.Decimal:
                case SADbType.Money:
                case SADbType.Numeric:
                case SADbType.SmallMoney:
                    parameter.Precision = Convert.ToByte(parameter.Size.ToString());
                    parameter.Size = ((parameter.Precision & 0xff) / 2) + 1;
                    break;
                case SADbType.UniqueIdentifier:
                    parameter.Size = 0x10;
                    break;
                case SADbType.NChar:
                case SADbType.NVarChar:
                case SADbType.NText:
                case SADbType.LongNVarchar:
                    parameter.Scale = 0;
                    break;
                case SADbType.Date:
                case SADbType.DateTime:
                case SADbType.DateTimeOffset:
                case SADbType.SmallDateTime:
                case SADbType.Time:
                case SADbType.TimeStamp:
                case SADbType.TimeStampWithTimeZone:
                    parameter.Scale = 6;
                    break;
            }
            return parameter;
        }

        public Key GetPrimaryKey(Table table)
        {
            if (table == null) throw new ArgumentNullException("table");
            return new Key(GetPrimaryKeys(table.ActualName).AsEnumerable()
                .Where(
                    row =>
                    row["TABLE_SCHEMA"].ToString() == table.Schema && row["TABLE_NAME"].ToString() == table.ActualName)
                    .OrderBy(row => Convert.ToInt64(row["ORDINAL_POSITION"]))
                    .Select(row => row["COLUMN_NAME"].ToString()));
        }

        public IEnumerable<ForeignKey> GetForeignKeys(Table table)
        {
            if (table == null) throw new ArgumentNullException("table");
            var groups = GetForeignKeys(table.ActualName)
                .Where(row =>
                    row["TABLE_SCHEMA"].ToString() == table.Schema && row["TABLE_NAME"].ToString() == table.ActualName)
                .GroupBy(row => row["CONSTRAINT_NAME"].ToString())
                .ToList();

            foreach (var group in groups)
            {
                yield return new ForeignKey(new ObjectName(group.First()["TABLE_SCHEMA"].ToString(), group.First()["TABLE_NAME"].ToString()),
                    group.Select(row => row["COLUMN_NAME"].ToString()),
                    new ObjectName(group.First()["UNIQUE_TABLE_SCHEMA"].ToString(), group.First()["UNIQUE_TABLE_NAME"].ToString()),
                    group.Select(row => row["UNIQUE_COLUMN_NAME"].ToString()));
            }
        }

        public string QuoteObjectName(string unquotedName)
        {
            if (unquotedName == null) throw new ArgumentNullException("unquotedName");
            if (unquotedName.StartsWith("[")) return unquotedName;

            return string.Concat("[", unquotedName, "]");
        }

        public string NameParameter(string baseName)
        {
            if (baseName == null) throw new ArgumentNullException("baseName");
            if (baseName.Length == 0) throw new ArgumentException("Base name must be provided");
            return (baseName.StartsWith(":")) ? baseName : ":" + baseName;
        }

        private DataTable GetColumnsDataTable(Table table)
        {
            var columnSelect =
                string.Format(
                    "select sc.column_name as name, (case when sc.[default]='autoincrement' then 1 else 0 end) as is_identity, "+
                    "sd.domain_name, (select type_name from sysusertype where type_id=sc.user_type) as type_name, "+
                    "(case when sc.width=32767 then 0 else sc.width end) as max_length from syscolumn sc "+
                    "join sysdomain sd on sd.domain_id=sc.domain_id "+
                    "join systable st on st.table_id=sc.table_id "+
                    "join sysuserperm sup on sup.user_id=st.creator "+
                    "where sup.user_name='{0}' and st.table_name='{1}' and st.table_type in ('VIEW', 'BASE') order by column_id",
                    table.Schema, table.ActualName);
            return SelectToDataTable(columnSelect);
        }

        private DataTable GetPrimaryKeys()
        {
            return SelectToDataTable(Properties.Resources.PrimaryKeySql);
        }

        private DataTable GetForeignKeys()
        {
            return SelectToDataTable(Properties.Resources.ForeignKeysSql);
        }

        private DataTable GetPrimaryKeys(string tableName)
        {
            return GetPrimaryKeys().AsEnumerable()
                .Where(
                    row => row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                .CopyToDataTable();
        }

        private EnumerableRowCollection<DataRow> GetForeignKeys(string tableName)
        {
            return GetForeignKeys().AsEnumerable()
                .Where(
                    row => row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.InvariantCultureIgnoreCase));
        }

        private DataTable SelectToDataTable(string sql)
        {
            var dataTable = new DataTable();
            using (var cn = ConnectionProvider.CreateConnection() as SAConnection)
            {
                using (var adapter = new SADataAdapter(sql, cn))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        private static SADbType? DbTypeFromDbTypeName(string typeName)
        {
            if (!String.IsNullOrEmpty(typeName))
            {
                return DbTypeLookup.GetSADbType(typeName);
            }
            return null;
        }

        public String GetDefaultSchema()
        {
            return "dba";
        }
    }
}
