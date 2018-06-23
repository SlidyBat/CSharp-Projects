using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;

public static class Sql
{
    private static MySqlConnection sql = null;

    public static void Init()
    {
        if(sql == null)
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.Server = "***REMOVED***";
            builder.Port = 3306;
            builder.UserID = "***REMOVED***";
            builder.Password = "***REMOVED***";
            builder.Database = "***REMOVED***";
            builder.SslMode = MySqlSslMode.None;

            sql = new MySqlConnection(builder.ToString());

            sql.Open();
        }
    }

    public static MySqlCommand Query(string queryString)
    {
        return new MySqlCommand(queryString, sql);
    }
}
