using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.IO;

namespace Trader
{
    public class DbUtils
    {
        public static SqlConnection OpenSqlConnection()
        {
            var c = JsonUtils.Parse(File.ReadAllText("config.json"));
            var connection = new SqlConnection(c["db"].ToString());
            connection.Open();
            return connection;
        }

        public static IEnumerable<T> Query<T>(IDbConnection con, string sql, object dbParams = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null)
        {
            return con.Query<T>(sql
                , dbParams
                , transaction
                , commandType: commandType);
        }

        public static int Execute(IDbConnection con, string sql, object dbParams = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null)
        {
            return con.Execute(sql
                , dbParams
                , transaction
                , commandType: commandType);
        }



    }
}