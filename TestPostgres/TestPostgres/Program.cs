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
                    Console.WriteLine("4. Добавить новую роль");
                    Console.WriteLine("5. Удалить роль");
                    Console.WriteLine("6. Вывести список ролей");
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
                        case "4":
                            Console.Write("Введите имя роли: ");
                            var role_name = Console.ReadLine();
                            AddRole(connection, role_name);
                            break;
                        case "5":
                            Console.Write("Введите ID роли для удаления: ");
                            var roleIdToDelete = Convert.ToInt32(Console.ReadLine());
                            DeleteRole(connection, roleIdToDelete);
                            break;
                        case "6":
                            ShowRoles(connection);
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

        private static void AddRole(NpgsqlConnection connection, string role_name)
        {
            var sql = "INSERT INTO Roles (role_id, role_name) VALUES (nextval('user_id_seq'), @Role)";
            connection.Execute(sql, new {Role =  role_name });
            Console.WriteLine("Новая роль успешно создана!");
        }

        private static void DeleteRole(NpgsqlConnection connection, int role_id)
        {
            var sql = "DELETE FROM Roles WHERE role_id = @RoleId";
            connection.Execute(sql, new { RoleId = role_id });
            Console.WriteLine("Роль удалена.");
        }

        private static void ShowRoles(NpgsqlConnection connection)
        {
            var sql = "SELECT * FROM Roles";
            var roles = connection.Query<Role>(sql);
            foreach (var role in roles)
            {
                Console.WriteLine($"ID: {role.role_id}, Role: {role.role_name}");
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

    public class Role
    {
        public int role_id { get; set; }
        public string role_name { get; set; }
    }
}
