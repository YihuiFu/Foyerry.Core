using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Foyerry.Core.Database
{
    public class MysqlDbClient : AbstractDbClient
    {
        private readonly string _connectionString;

        public MysqlDbClient(string connectionString)
        {
            _connectionString = connectionString;
        }
        public override string ConnectionString
        {
            get { return _connectionString; }
        }
        protected override DbProviderFactory Factory
        {
            get { return MySql.Data.MySqlClient.MySqlClientFactory.Instance; }
        }
    }
}
