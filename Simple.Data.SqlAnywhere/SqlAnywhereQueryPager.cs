﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Simple.Data.Ado;

namespace Simple.Data.SqlAnywhere
{
    public class SqlAnywhereQueryPager : IQueryPager
    {
        private static readonly Regex ColumnExtract = new Regex(@"SELECT\s*(.*)\s*(FROM.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex SelectMatch = new Regex(@"^SELECT\s*", RegexOptions.IgnoreCase);

        public IEnumerable<string> ApplyLimit(string sql, int take)
        {
            yield return SelectMatch.Replace(sql, match => match.Value + " TOP " + take + " ");
        }

        private readonly bool supportsCommonTableExpressions;

        public SqlAnywhereQueryPager(Boolean supportsCommonTableExpressions)
        {
            this.supportsCommonTableExpressions = supportsCommonTableExpressions;
        }

        public IEnumerable<String> ApplyPaging(string sql, int skip, int take)
        {
            var builder = new StringBuilder();

            var match = ColumnExtract.Match(sql);
            var columns = match.Groups[1].Value.Trim();
            var fromEtc = match.Groups[2].Value.Trim();
            var orderBy = ExtractOrderBy(columns, ref fromEtc);
            var withTable = "__Data";

            var rangeMin = skip + 1;
            var rangeMax = take + skip;
            if (rangeMax < 0)
            {
                rangeMax = 2147483646;
            }
            var rangeFormat = default(String);
            if ((rangeMin > 1) && (rangeMax < 2147483646))
            {
                rangeFormat = "BETWEEN {0} AND {1}";
            }
            else if (rangeMin > 1)
            {
                rangeFormat = ">= {0}";
            }
            else
            {
                rangeFormat = "<= {1}";
            }
            var range = String.Format(rangeFormat, rangeMin, rangeMax);

            if (supportsCommonTableExpressions)
            {
                builder.AppendFormat("WITH {0} AS (SELECT ", withTable);
                builder.Append(columns);
                builder.AppendFormat(", ROW_NUMBER() OVER({0}) AS [_#_]", orderBy);
                builder.AppendLine();
                builder.Append(fromEtc);
                builder.AppendLine(")");
                builder.AppendFormat("SELECT {0} FROM {1} WHERE [_#_] {2}", DequalifyColumns(columns), withTable, range);
                yield return builder.ToString();
            }
            else
            {
                withTable = "#__Data";
                builder.Append("SELECT ");
                if (rangeMax < 32768)
                {
                    builder.AppendFormat("TOP {0} ", take + skip);
                }
                builder.Append(columns);
                if ((rangeMin > 1) || (rangeMax >= 32768))
                {
                    builder.Append(", CAST(NUMBER() AS INT) AS [_#_]");
                    builder.AppendLine();
                    builder.AppendFormat("INTO {0}", withTable);
                }
                builder.AppendLine();
                builder.Append(fromEtc);
                builder.AppendLine();
                builder.Append(orderBy);
                yield return builder.ToString();
                if ((rangeMin > 1) || (rangeMax >= 32768))
                {
                    yield return String.Format("SELECT {0} FROM {1} WHERE [_#_] {2}", DequalifyColumns(columns), withTable, range);
                    yield return String.Format("DROP TABLE {0}", withTable);
                }
            }
        }

        private static string DequalifyColumns(string original)
        {
            var q = from part in original.Split(',')
                    select part.Substring(Math.Max(part.LastIndexOf('.') + 1, part.LastIndexOf('[')));
            return string.Join(",", q);
        }

        private static string ExtractOrderBy(string columns, ref string fromEtc)
        {
            string orderBy;
            int index = fromEtc.IndexOf("ORDER BY", StringComparison.InvariantCultureIgnoreCase);
            if (index > -1)
            {
                orderBy = fromEtc.Substring(index).Trim();
                fromEtc = fromEtc.Remove(index).Trim();
            }
            else
            {
                orderBy = "ORDER BY " + columns.Split(',').First().Trim();

                var aliasIndex = orderBy.IndexOf(" AS [", StringComparison.InvariantCultureIgnoreCase);

                if (aliasIndex > -1)
                {
                    orderBy = orderBy.Substring(0, aliasIndex);
                }
            }
            return orderBy;
        }
    }
}
