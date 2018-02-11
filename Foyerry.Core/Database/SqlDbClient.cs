using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Foyerry.Core.Database
{
    public class SqlDbClient : AbstractDbClient
    {
        private readonly string _connectionString;

        public SqlDbClient(string connectionString)
        {
            _connectionString = connectionString;
        }
        public override string ConnectionString
        {
            get { return _connectionString; }
        }
        protected override DbProviderFactory Factory
        {
            get { return SqlClientFactory.Instance; }
        }
    }
}
