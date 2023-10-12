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
        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Files");
        foreach(var file in files)
        {
            using(StreamReader sr = new StreamReader(file))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string[] fields = sr.ReadLine().Split('|');
                    using(SqlConnection conn = new SqlConnection(sqlConStr))
                    {
                        conn.Open();
                        string inlineSQL = $@"INSERT INTO [dbo].[Produce] ([Name],[Location],[Price],[UoM],[Sell_by_Date]) VALUES ('{fields[0]}','{fields[1]}','{fields[2]}','{fields[3]}','{fields[4]}')";
                        using (var command = new SqlCommand(inlineSQL, conn))
                        {
                            var query = command.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }
        }
        using (SqlConnection conn = new SqlConnection(sqlConStr))
        {
            conn.Open();
            string inlineSQL = @"Select * from Produce";
            using (var command = new SqlCommand(inlineSQL, conn))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var value = $"{reader.GetValue(0)},{reader.GetValue(1)},{reader.GetValue(2)},{reader.GetValue(3)},{reader.GetValue(4)}";
                    Console.WriteLine(value);
                }
                reader.Close();
            }
            conn.Close();
        }
    }
}