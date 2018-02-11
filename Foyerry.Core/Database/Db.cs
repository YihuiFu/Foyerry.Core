using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Foyerry.Core.Database
{
    public class Db
    {
        private static readonly Dictionary<DbIdentity, IDbClient> KnownClients = new Dictionary<DbIdentity, IDbClient>();

        public static IDbClient Acg_News { get { return GetClient("Acg_News", DatabaseType.SqlServer); } }

        public static IDbClient GetClient(string dbName, DatabaseType databaseType, string connectionString = null)
        {

            var id = new DbIdentity(dbName, databaseType);

            IDbClient client;
            if (KnownClients.TryGetValue(id, out client))
                return client;

            lock (KnownClients)
            {
                if (KnownClients.TryGetValue(id, out client))
                    return client;

                var conn = connectionString ?? GetConnectionString(dbName, databaseType);

                if (databaseType == DatabaseType.SqlServer)
                {
                    client = new SqlDbClient(conn);
                }
                else
                {
                    client = new MysqlDbClient(conn);
                }

                KnownClients[id] = client;
                return client;
            }
        }

        public static string GetConnectionString(string dbName, DatabaseType databaseType)
        {
            //TODO:
            return "";
        }

        private struct DbIdentity : IEquatable<DbIdentity>
        {
            private readonly string _name;
            private readonly DatabaseType _type;

            public DbIdentity(string name, DatabaseType type)
            {
                _name = name;
                _type = type;
            }

            public bool Equals(DbIdentity other)
            {
                return _name == other._name && _type == other._type;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is DbIdentity))
                    return false;

                return Equals((DbIdentity)obj);
            }

            public override int GetHashCode()
            {
                return _name.GetHashCode() ^ _type.GetHashCode();
            }
        }
    }

    public enum DatabaseType
    {
        SqlServer = 1,
        MySql = 2,
    }
}
