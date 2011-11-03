namespace Simple.Data.SqlAnywhere
{
    using System.ComponentModel.Composition;
    using System.Data;
    using System.Data.SqlClient;
    using Ado;
    using Ado.Schema;
    using iAnywhere.Data.SQLAnywhere;

    [Export(typeof(IDbParameterFactory))]
    public class SqlAnywhereParameterFactory : IDbParameterFactory
    {
        public IDbDataParameter CreateParameter(string name)
        {
            return new SAParameter
                {
                    ParameterName = name
                };
        }
        
        public IDbDataParameter CreateParameter(string name, Column column)
        {
            var sqlAnywhereColumn = (SqlAnywhereColumn) column;
            return new SAParameter(name, sqlAnywhereColumn.SADbType, column.MaxLength, column.ActualName);
        }

        public IDbDataParameter CreateParameter(string name, DbType dbType, int maxLength)
        {
            IDbDataParameter parameter = new SAParameter
                       {
                           ParameterName = name,
                           Size = maxLength
                       };
            parameter.DbType = dbType;
            return parameter;
        }
    }
}