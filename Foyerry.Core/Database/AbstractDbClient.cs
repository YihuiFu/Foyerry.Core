using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Foyerry.Core.Database
{
    public abstract class AbstractDbClient : IDbClient
    {
        public virtual int DefaultTimeout { get; set; } = 0;

        public abstract string ConnectionString { get; }

        protected abstract DbProviderFactory Factory { get; }

        public virtual object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        public virtual int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        public virtual DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return FillDataTable(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        public virtual DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return FillDataSet(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        public virtual bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return Scalar(sql, parameters, commandType, timeout) != null;
        }

        public virtual IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand cmd = null;

            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd?.Parameters.Clear();

                //获取reader失败时，关闭连接
                if (reader == null && connection != null)
                    CloseConnection(connection);
            }

            return YieldRows(connection, reader);
        }

        public DbParameter CreateParameter()
        {
            return Factory.CreateParameter();
        }

        protected virtual DbConnection CreateConnection()
        {
            var connection = Factory.CreateConnection();

            if (connection == null)
                throw new NotSupportedException("获取数据库连接失败。");

            connection.ConnectionString = ConnectionString;
            return connection;
        }

        protected virtual DbCommand CreateCommand(string commandText,
            DbConnection connection, IEnumerable<DbParameter> parameters,
            CommandType commandType, int timeout)
        {
            var cmd = connection.CreateCommand();

            cmd.CommandType = commandType;
            cmd.CommandText = commandText;

            // timeout 为0时套用默认超时。
            cmd.CommandTimeout = timeout == 0 ? DefaultTimeout : timeout;

            if (parameters != null)
            {
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);
            }

            return cmd;
        }

        protected virtual void OpenConnection(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        protected virtual void CloseConnection(DbConnection connection)
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        private DbConnection CreateAndOpenConnection()
        {
            var connection = CreateConnection();
            OpenConnection(connection);
            return connection;
        }

        private IEnumerable<IDataRecord> YieldRows(DbConnection connection, DbDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    yield return reader;
                }
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        private DataTable FillDataTable(DbCommand command)
        {
            var dataAdapter = CreateDataAdapter();
            var dataTable = new DataTable();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        private DataSet FillDataSet(DbCommand command)
        {
            var dataAdapter = CreateDataAdapter();
            var dataSet = new DataSet();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dataSet);
            return dataSet;
        }

        private DbDataAdapter CreateDataAdapter()
        {
            var dataAdapter = Factory.CreateDataAdapter();

            if (dataAdapter == null)
                throw new NotSupportedException("Cannot create a data-adapter from the underlying DbProviderFactory.");

            return dataAdapter;
        }
    }
}
