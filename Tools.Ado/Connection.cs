using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace Tools.Ado
{
    public class Connection
    {
        private readonly string _connectionString;

        public Connection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int ExecuteNonQuery(Command command)
        {
            using(SqlConnection sqlConnection = CreateConnection(_connectionString))
            {
                using(SqlCommand sqlCommand = CreateCommand(sqlConnection, command)) 
                {
                    sqlConnection.Open();

                    return sqlCommand.ExecuteNonQuery();
                }                
            }            
        }        

        public object? ExecuteScalar(Command command)
        {
            using (SqlConnection sqlConnection = CreateConnection(_connectionString))
            {
                using (SqlCommand sqlCommand = CreateCommand(sqlConnection, command))
                {
                    sqlConnection.Open();

                    object? result = sqlCommand.ExecuteScalar();

                    return result is DBNull ? null : result;
                }
            }
        }

        public DataTable GetDataTable(Command command)
        {
            using (SqlConnection sqlConnection = CreateConnection(_connectionString))
            {
                using (SqlCommand sqlCommand = CreateCommand(sqlConnection, command))
                {
                    using(SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        DataTable dataTable = new DataTable();

                        sqlDataAdapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<SqlDataReader, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            using (SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = _connectionString;

                using (SqlCommand sqlCommand = CreateCommand(sqlConnection, command))
                {
                    sqlConnection.Open();

                    using(SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            yield return selector(reader);
                        }
                    }
                }
            }
        }

        private static SqlConnection CreateConnection(string connectionString)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        private static SqlCommand CreateCommand(SqlConnection sqlConnection, Command command)
        {
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            sqlCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
                sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach(KeyValuePair<string, object> kvp in command.Parameters)
            {
                SqlParameter sqlParameter = sqlCommand.CreateParameter();
                sqlParameter.ParameterName = kvp.Key;
                sqlParameter.Value = kvp.Value;

                sqlCommand.Parameters.Add(sqlParameter);
            }

            return sqlCommand;
        }
    }
}
