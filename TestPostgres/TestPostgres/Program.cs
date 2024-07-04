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


                var Flag = true;
                while (Flag)
                {
                    Console.WriteLine("\n1. Добавить пользователя");
                    Console.WriteLine("2. Удалить пользователя");
                    Console.WriteLine("3. Вывести информацию о пользователях");
                    Console.Write("Выберите действие: ");
                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Введите имя пользователя: ");
                            var username = Console.ReadLine();
                            AddUser(connection, username);
                            break;
                        case "2":
                            Console.Write("Введите ID пользователя для удаления: ");
                            var userIdToDelete = Convert.ToInt32(Console.ReadLine());
                            DeleteUser(connection, userIdToDelete);
                            break;
                        case "3":
                            ShowUsers(connection);
                            break;
                        default:
                            Console.WriteLine("Неверный выбор.");
                            Flag = false;
                            break;
                    }
                }
            }
            Console.ReadKey();
        }
        private static void AddUser(NpgsqlConnection connection, string username)
        {
            var sql = "INSERT INTO Users (user_id, username) VALUES (nextval('user_id_seq'), @Username)";
            connection.Execute(sql, new { Username = username });
            Console.WriteLine("Пользователь успешно добавлен.");
        }

        private static void DeleteUser(NpgsqlConnection connection, int userId)
        {
            var sql = "DELETE FROM Users WHERE user_id = @UserId";
            connection.Execute(sql, new { UserId = userId });
            Console.WriteLine("Пользователь успешно удален.");
        }
        private static void ShowUsers(NpgsqlConnection connection)
        {
            var sql = "SELECT * FROM Users";
            var users = connection.Query<User>(sql);
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.user_id}, Username: {user.username}");
            }
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
    public class User
    {
        public int user_id { get; set; }
        public string username { get; set; }
    }
}
