using System.Data.SqlClient;
using System.IO;

internal class Program
{
    static void Main(string[] args)
    {
        SqlConnectionStringBuilder mySqlConBldr = new SqlConnectionStringBuilder();
        mySqlConBldr["server"] = @"(localdb)\MSSQLLocalDB";
        mySqlConBldr["Trusted_Connection"] = true;
        mySqlConBldr["Integrated Security"] = "SSPI";
        mySqlConBldr["Initial Catalog"] = "PROG260FA23";
        string sqlConStr = mySqlConBldr.ToString();
        Console.WriteLine(sqlConStr);

        using(SqlConnection conn = new SqlConnection(sqlConStr))
        {
            conn.Open();

        }
    }
}