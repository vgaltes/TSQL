using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace TSQL
{
    public static class QueryHelper
    {
        private const string ConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TSQLV4;Integrated Security=True";

        public static IEnumerable<dynamic> ExecuteQuery(string query)
        {
            IEnumerable<dynamic> results = null;
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                results = sqlConnection.Query(query);
                sqlConnection.Close();
            }

            return results;
        }
    }
}