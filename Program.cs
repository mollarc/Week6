using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

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
            if (file.Contains("_Output"))
            {
                break;
            }
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
            string inlineSQL = $@"DELETE FROM [dbo].[Produce] WHERE [Sell_by_Date] < GETDATE()"; //https://learn.microsoft.com/en-us/sql/t-sql/functions/getdate-transact-sql?view=sql-server-ver16 Get date
            using (var command = new SqlCommand(inlineSQL, conn))
            {
                var query = command.ExecuteNonQuery();
            }
            conn.Close();
        }
        using (SqlConnection conn = new SqlConnection(sqlConStr))
        {
            conn.Open();
            string inlineSQL = @"UPDATE [dbo].[Produce] SET [Location] = Replace([Location],'F','Z')"; //https://www.w3schools.com/sql/func_sqlserver_replace.asp For Replace Function 
            using (var command = new SqlCommand(inlineSQL, conn))
            {
                var query = command.ExecuteNonQuery();
            }
            conn.Close();
        }
        using (SqlConnection conn = new SqlConnection(sqlConStr))
        {
            conn.Open();
            string inlineSQL = @"UPDATE [dbo].[Produce] SET [Price] = [Price] + 1";
            using (var command = new SqlCommand(inlineSQL, conn))
            {
                var query = command.ExecuteNonQuery();
            }
            conn.Close();
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
                    var value = $"{reader.GetValue(0)},{reader.GetValue(1)},{reader.GetValue(2)},{reader.GetValue(3)},{reader.GetValue(4)},{reader.GetValue(5)}";
                    Console.WriteLine(value);
                    foreach (var file in files)
                    {
                        if (file.Contains("_Output"))
                        {
                            break;
                        }
                        using (FileStream fs = File.Create($"{file.Substring(0, file.Length - 4)}_Output.txt"))
                        {
                            using(StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.WriteLine(value);
                            }
                        }
                    }
                }
                reader.Close();
            }
            conn.Close();
        }
    }
}