using Dapper;
using Npgsql;
using System;
using System.Diagnostics;
using System.IO;

namespace TestPostgres
{
    internal class Program
    {
        static void Main(string[] args)
        {  
            Console.Title = "Terminal PostrgesSQL TestTimeManagement db";

            var connectionString = GetConnectionString();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();


                connection.Query<string>("Select username from Users;");


            }

            //Console.WriteLine("Hello, World!");


            Console.ReadKey();
        }

        // Метод для получения строки подключения из конфигурационного файла
        private static string GetConnectionString()
        {
            // Путь к файлу конфигурации
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            // Читаем файл конфигурации
            var config = File.ReadAllText(configFilePath);

            // Парсим JSON
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(config);

            // Получаем строку подключения
            return jsonObject.ConnectionStrings.TestTimeManagement;
        }
    }
}
