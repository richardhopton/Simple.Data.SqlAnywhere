namespace Simple.Data.SqlAnywhere
{
    using System.Data;
    using Ado.Schema;
    using iAnywhere.Data.SQLAnywhere;

    public class SqlAnywhereColumn : Column
    {
        private readonly SADbType _saDbType;

        public SqlAnywhereColumn(string actualName, Table table) : base(actualName, table)
        {
        }

        public SqlAnywhereColumn(string actualName, Table table, SADbType saDbType) : base(actualName, table)
        {
            _saDbType = saDbType;
        }

        public SqlAnywhereColumn(string actualName, Table table, bool isIdentity) : base(actualName, table, isIdentity)
        {
        }

        public SqlAnywhereColumn(string actualName, Table table, bool isIdentity, SADbType saDbType, int maxLength)
            : base(actualName, table, isIdentity, DbTypeLookup.GetDbType(saDbType) ?? default(DbType), maxLength)
        {
            _saDbType = saDbType;
        }

        public SADbType SADbType
        {
            get { return _saDbType; }
        }

        public override bool IsBinary
        {
            get
            {
                return SADbType == SADbType.Binary || SADbType == SADbType.Image ||
                       SADbType == SADbType.VarBinary || SADbType == SADbType.LongBinary;
            }
        }
    }
}