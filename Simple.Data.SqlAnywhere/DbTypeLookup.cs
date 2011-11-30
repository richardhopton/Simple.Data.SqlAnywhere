namespace Simple.Data.SqlAnywhere
{
    using System.Collections.Generic;
    using System.Data;
    using iAnywhere.Data.SQLAnywhere;
    using System;

    internal static class DbTypeLookup
    {
        private static readonly String[] SADbTypeNames = new[] { 
           "bigint", "binary", "bit", "char", "date", "datetime", "datetimeoffset", "decimal", "double", "float", "image", "integer", "long binary", "long nvarchar", "long varbit", "long varchar", 
           "money", "nchar", "ntext", "numeric", "nvarchar", "real", "smalldatetime", "smallint", "smallmoney", "sysname", "text", "time", "timestamp", "timestamp with time zone", "tinyint", "uniqueidentifier", 
           "uniqueidentifierstr", "unsigned bigint", "unsigned int", "unsigned smallint", "varbinary", "varbit", "varchar", "xml", "st_geometry"
        };
        private static readonly DbType[] DbTypes = new[] { 
            DbType.Int64, DbType.Binary, DbType.Boolean, DbType.AnsiStringFixedLength, DbType.Date, DbType.DateTime, DbType.DateTimeOffset, DbType.Decimal, DbType.Double, DbType.Single, DbType.Binary, DbType.Int32, DbType.Binary, DbType.String, DbType.AnsiString, DbType.AnsiString, 
            DbType.Currency, DbType.StringFixedLength, DbType.String, DbType.Decimal, DbType.String, DbType.Single, DbType.DateTime, DbType.Int16, DbType.Currency, DbType.AnsiString, DbType.AnsiString, DbType.Time, DbType.DateTime, DbType.DateTimeOffset, DbType.Byte, DbType.Guid, 
            DbType.AnsiStringFixedLength, DbType.UInt64, DbType.UInt32, DbType.UInt16, DbType.Binary, DbType.AnsiString, DbType.AnsiString, DbType.AnsiString, DbType.AnsiString
        };
        private static readonly SADbType[] SADbTypes = new[] { 
            SADbType.BigInt, SADbType.Binary, SADbType.Bit, SADbType.Char, SADbType.Date, SADbType.DateTime, SADbType.DateTimeOffset, SADbType.Decimal, SADbType.Double, SADbType.Float, SADbType.Image, SADbType.Integer, SADbType.LongBinary, SADbType.LongNVarchar, SADbType.LongVarbit, SADbType.LongVarchar, 
            SADbType.Money, SADbType.NChar, SADbType.NText, SADbType.Numeric, SADbType.NVarChar, SADbType.Real, SADbType.SmallDateTime, SADbType.SmallInt, SADbType.SmallMoney, SADbType.SysName, SADbType.Text, SADbType.Time, SADbType.TimeStamp, SADbType.TimeStampWithTimeZone, SADbType.TinyInt, SADbType.UniqueIdentifier, 
            SADbType.UniqueIdentifierStr, SADbType.UnsignedBigInt, SADbType.UnsignedInt, SADbType.UnsignedSmallInt, SADbType.VarBinary, SADbType.VarBit, SADbType.VarChar, SADbType.Xml, SADbType.LongVarchar
        };
        private static readonly Type[] ClrTypes = new[]{
            typeof(Int64), typeof(Byte[]), typeof(Boolean), typeof(String), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(Decimal), typeof(Double), typeof(Single), typeof(Byte[]), typeof(Int32), typeof(Byte[]), typeof(String), typeof(String), typeof(String),
            typeof(Decimal), typeof(String), typeof(String), typeof(Decimal), typeof(String), typeof(Single), typeof(DateTime), typeof(Int16), typeof(Decimal), typeof(String), typeof(String), typeof(TimeSpan), typeof(DateTime), typeof(DateTime), typeof(Byte), typeof(Guid),
            typeof(String), typeof(UInt64), typeof(UInt32), typeof(UInt16), typeof(Byte[]), typeof(String), typeof(String), typeof(String), typeof(String) 
        };

        private static Boolean FindTargetValue<TSource, TTarget>(TSource sourceValue, TSource[] sourceArray, TTarget[] targetArray, out TTarget target, Func<TSource, TSource, Boolean> matches)
        {
            var index = -1;
            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (matches(sourceArray[i], sourceValue))
                {
                    index = i;
                    break;
                }
            }
            if ((index > -1) && (index < targetArray.Length))
            {
                target = targetArray[index];
                return true;
            }
            target = default(TTarget);
            return false;
        }

        public static SADbType? GetSADbType(string typeName)
        {
            var target = default(SADbType);
            if (FindTargetValue(typeName, SADbTypeNames, SADbTypes, out target,
                    (x, y) => String.Equals(x, y, StringComparison.OrdinalIgnoreCase)))
            {
                return target;
            }
            return null;
        }

        public static DbType? GetDbType(this SADbType saDbType)
        {
            var target = default(DbType);
            if (FindTargetValue(saDbType, SADbTypes, DbTypes, out target, (x, y) => x == y))
            {
                return target;
            }
            return null;
        }

        public static Type GetClrType(this SADbType saDbType)
        {
            var target = default(Type);
            FindTargetValue(saDbType, SADbTypes, ClrTypes, out target, (x, y) => x == y);
            return target;
        }
    }
}