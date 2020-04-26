using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace SAPC
{
    public class MySql
    {
        private static MySqlConnection connection;
        private MySql()
        {

        }
        public static void Init()
        {
            Console.WriteLine(">>> Connecting to database");
            try
            {
                connection = new MySqlConnection("server=localhost; database=pursuit; user=root; password=;");
                connection.Open();
                Console.WriteLine(">>> Connected to database");
            }
            catch(Exception e)
            {
               Console.WriteLine(">>> Failed to connect to database. Reason: " + e.Message);
            }
        }
        public static MySqlDataReader Reader(string query)
        {
            return new MySqlCommand(query, connection).ExecuteReader();
        }
        public static void Query(string query)
        {
            new MySqlCommand(query, connection).ExecuteNonQuery();
        }
    }
}