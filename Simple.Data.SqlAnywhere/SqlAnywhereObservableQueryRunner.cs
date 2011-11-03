﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.SqlAnywhere
{
    using System.ComponentModel.Composition;
    using System.Data;
    using System.Data.SqlClient;
    using Ado;
    using iAnywhere.Data.SQLAnywhere;

    [Export(typeof(IObservableQueryRunner))]
    public class SqlAnywhereObservableQueryRunner : IObservableQueryRunner
    {
        public IObservable<IDictionary<string, object>> Run(IDbCommand command, IDbConnection connection, IDictionary<string, int> index)
        {
            return new SqlAnywhereObservable(connection as SAConnection, command as SACommand, index);
        }

        class SqlAnywhereObservable : IObservable<IDictionary<string,object>>
        {
            private readonly SAConnection _connection;
            private readonly SACommand _command;
            private IDictionary<string, int> _index;

            public SqlAnywhereObservable(SAConnection connection, SACommand command, IDictionary<string,int> index)
            {
                if (connection == null) throw new ArgumentNullException("connection");
                if (command == null) throw new ArgumentNullException("command");
                _connection = connection;
                _command = command;
                _index = index;
            }

            public IDisposable Subscribe(IObserver<IDictionary<string, object>> observer)
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                _command.BeginExecuteReader(ExecuteReaderCompleted, observer);

                return new ActionDisposable(() =>
                                                {
                                                    using (_connection) using (_command) { }
                                                });
            }

            private void ExecuteReaderCompleted(IAsyncResult ar)
            {
                var observer = ar.AsyncState as IObserver<IDictionary<string, object>>;
                if (observer == null) throw new InvalidOperationException();
                try
                {
                    using (var reader = _command.EndExecuteReader(ar))
                    {
                        if (_index == null) _index = reader.CreateDictionaryIndex();
                        while (reader.Read())
                        {
                            observer.OnNext(reader.ToDictionary(_index));
                        }
                    }
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            }
        }
    }
}
