using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.SqlAnywhere
{
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;
    using Ado;
    using System.Data;

    [Export(typeof(CommandOptimizer))]
    public class SqlAnywhereCommandOptimizer : CommandOptimizer
    {
        public override IDbCommand OptimizeFindOne(IDbCommand command)
        {
            command.CommandText = Regex.Replace(command.CommandText, "^SELECT ", "SELECT TOP 1 ",
                                                RegexOptions.IgnoreCase);
            return command;
        }
    }
}
