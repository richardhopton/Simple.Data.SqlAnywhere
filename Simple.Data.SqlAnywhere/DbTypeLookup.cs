namespace Simple.Data.SqlAnywhere
{
    using System.Collections.Generic;
    using System.Data;
    using iAnywhere.Data.SQLAnywhere;
    using System;

    internal static class DbTypeLookup
    {
       private static readonly Dictionary<string, SADbType> SADbTypeLookup = new Dictionary<string, SADbType>
                                                                                    {
                                                                                        {"bigint", SADbType.BigInt},
                                                                                        {"binary", SADbType.Binary},
                                                                                        {"bit", SADbType.Bit},
                                                                                        {"char", SADbType.Char},
                                                                                        {"date", SADbType.Date},
                                                                                        {"datetime", SADbType.DateTime},
                                                                                        {"datetimeoffset", SADbType.DateTimeOffset},
                                                                                        {"decimal", SADbType.Decimal},
                                                                                        {"double", SADbType.Double},
                                                                                        {"float", SADbType.Float},
                                                                                        {"image", SADbType.Image},
                                                                                        {"integer", SADbType.Integer},
                                                                                        {"long binary", SADbType.LongBinary},
                                                                                        {"long nvarchar", SADbType.LongNVarchar},
                                                                                        {"long varbit", SADbType.LongVarbit},
                                                                                        {"long varchar", SADbType.LongVarchar},
                                                                                        {"money", SADbType.Money},
                                                                                        {"nchar", SADbType.NChar},
                                                                                        {"ntext", SADbType.NText},
                                                                                        {"numeric", SADbType.Numeric},
                                                                                        {"nvarchar", SADbType.NVarChar},
                                                                                        {"real", SADbType.Real},
                                                                                        {"smalldatetime", SADbType.SmallDateTime},
                                                                                        {"smallint", SADbType.SmallInt},
                                                                                        {"smallmoney", SADbType.SmallMoney},
                                                                                        {"st_geometry", SADbType.LongVarchar},
                                                                                        {"sysname", SADbType.SysName},
                                                                                        {"text", SADbType.Text},
                                                                                        {"time", SADbType.Time},
                                                                                        {"timestamp", SADbType.TimeStamp},
                                                                                        {"timestamp with time zone", SADbType.TimeStampWithTimeZone},
                                                                                        {"tinyint", SADbType.TinyInt},
                                                                                        {"uniqueidentifier", SADbType.UniqueIdentifier},
                                                                                        {"uniqueidentifierstr", SADbType.UniqueIdentifierStr},
                                                                                        {"unsigned bigint", SADbType.UnsignedBigInt},
                                                                                        {"unsigned integer", SADbType.UnsignedInt},
                                                                                        {"unsigned smallint", SADbType.UnsignedSmallInt},
                                                                                        {"varbinary", SADbType.VarBinary},
                                                                                        {"varbit", SADbType.VarBit},
                                                                                        {"varchar", SADbType.VarChar},
                                                                                        {"xml", SADbType.Xml},
                                                                                    };

        public static SADbType? GetSADbType(string typeName)
        {
            var result = default(SADbType);
            if (SADbTypeLookup.TryGetValue(typeName, out result))
            {
                return result;
            }
            return null;
        }
    }
}