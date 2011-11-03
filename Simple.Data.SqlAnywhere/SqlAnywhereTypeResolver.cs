using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using iAnywhere.Data.SQLAnywhere;

namespace Simple.Data.SqlAnywhere
{
    static class SqlAnywhereTypeResolver
    {
        private static readonly Dictionary<SADbType, Type> ClrTypes = new Dictionary<SADbType, Type>
                                                                        {
                                                                           {SADbType.BigInt, typeof(Int64)},
                                                                           {SADbType.Binary, typeof(Byte[])},
                                                                           {SADbType.Bit, typeof(Boolean)},
                                                                           {SADbType.Char, typeof(String)},
                                                                           {SADbType.Date, typeof(DateTime)},
                                                                           {SADbType.DateTime, typeof(DateTime)},
                                                                           {SADbType.DateTimeOffset, typeof(DateTimeOffset)},
                                                                           {SADbType.Decimal, typeof(Decimal)},
                                                                           {SADbType.Double, typeof(Double)},
                                                                           {SADbType.Float, typeof(Single)},
                                                                           {SADbType.Image, typeof(Byte[])},
                                                                           {SADbType.Integer, typeof(Int32)},
                                                                           {SADbType.LongBinary, typeof(Byte[])},
                                                                           {SADbType.LongNVarchar, typeof(String)},
                                                                           {SADbType.LongVarbit, typeof(String)},
                                                                           {SADbType.LongVarchar, typeof(String)},
                                                                           {SADbType.Money, typeof(Decimal)},
                                                                           {SADbType.NChar, typeof(String)},
                                                                           {SADbType.NText, typeof(String)},
                                                                           {SADbType.Numeric, typeof(Decimal)},
                                                                           {SADbType.NVarChar, typeof(String)},
                                                                           {SADbType.Real, typeof(Single)},
                                                                           {SADbType.SmallDateTime, typeof(DateTime)},
                                                                           {SADbType.SmallInt, typeof(Int16)},
                                                                           {SADbType.SmallMoney, typeof(Decimal)},
                                                                           {SADbType.SysName, typeof(String)},
                                                                           {SADbType.Text, typeof(String)},
                                                                           {SADbType.Time, typeof(TimeSpan)},
                                                                           {SADbType.TimeStamp, typeof(DateTime)},
                                                                           {SADbType.TimeStampWithTimeZone, typeof(DateTimeOffset)},
                                                                           {SADbType.TinyInt, typeof(Byte)},
                                                                           {SADbType.UniqueIdentifier, typeof(Guid)},
                                                                           {SADbType.UniqueIdentifierStr, typeof(String)},
                                                                           {SADbType.UnsignedBigInt, typeof(UInt64)},
                                                                           {SADbType.UnsignedInt, typeof(UInt32)},
                                                                           {SADbType.UnsignedSmallInt, typeof(UInt16)},
                                                                           {SADbType.VarBinary, typeof(Byte[])},
                                                                           {SADbType.VarBit, typeof(String)},
                                                                           {SADbType.VarChar, typeof(String)},
                                                                           {SADbType.Xml, typeof(String)},
                                                                        };

        public static Type GetClrType(SADbType saDBType)
        {
            Type clrType;
            return ClrTypes.TryGetValue(saDBType, out clrType) ? clrType : typeof(object);
        }
    }
}