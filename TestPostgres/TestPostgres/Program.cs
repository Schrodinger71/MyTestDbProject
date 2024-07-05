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
                Console.WriteLine("Добро пожаловать в консольное приложение по управлению базой данных TestTimeManagement!");

                ShowMenu();

                var isRunning = true;
                while (isRunning)
                {
                    

                    Console.Write("\nВыберите действие: ");
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
                            var roleName = Console.ReadLine();
                            AddRole(connection, roleName);
                            break;
                        case "5":
                            Console.Write("Введите ID роли для удаления: ");
                            var roleIdToDelete = Convert.ToInt32(Console.ReadLine());
                            DeleteRole(connection, roleIdToDelete);
                            break;
                        case "6":
                            ShowRoles(connection);
                            break;
                        case "7":
                            Console.Write("Введите ID пользователя: ");
                            var userId = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Введите ID роли: ");
                            var roleId = Convert.ToInt32(Console.ReadLine());
                            AddUserToRole(connection, userId, roleId);
                            break;
                        case "8":
                            ShowUsersWithRoles(connection);
                            break;
                        case "9":
                            CountUsersInRoles(connection);
                            break;
                        case "10":
                            Console.Write("Введите ID пользователя: ");
                            var deleteUserId = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Введите ID роли: ");
                            var deleteRoleId = Convert.ToInt32(Console.ReadLine());
                            RemoveUserFromRole(connection, deleteUserId, deleteRoleId);
                            break;
                        case "11":
                            ShowUsersAndRoles(connection);
                            break;
                        case "12":
                            ShowMenu();
                            break;
                        case "13":
                            isRunning = false;
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Пожалуйста, выберите действие из меню.");
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
        private static void AddUserToRole(NpgsqlConnection connection, int userId, int roleId)
        {
            var sql = "INSERT INTO UserRoles (user_id, role_id) VALUES (@UserId, @RoleId)";
            connection.Execute(sql, new { UserId = userId, RoleId = roleId });
            Console.WriteLine("Пользователь успешно добавлен к роли.");
        }
        private static void ShowUsersWithRoles(NpgsqlConnection connection)
        {
            var sql = "SELECT Users.username, Roles.role_name FROM Users " +
                      "JOIN UserRoles ON Users.user_id = UserRoles.user_id " +
                      "JOIN Roles ON UserRoles.role_id = Roles.role_id";
            var userRoles = connection.Query(sql);
            foreach (var userRole in userRoles)
            {
                Console.WriteLine($"Пользователь: {userRole.username}, Роль: {userRole.role_name}");
            }
        }
        private static void CountUsersInRoles(NpgsqlConnection connection)
        {
            var sql = "SELECT Roles.role_name, COUNT(UserRoles.user_id) AS user_count FROM Roles " +
                      "LEFT JOIN UserRoles ON Roles.role_id = UserRoles.role_id " +
                      "GROUP BY Roles.role_name";
            var roleUserCounts = connection.Query(sql);
            foreach (var roleUserCount in roleUserCounts)
            {
                Console.WriteLine($"Роль: {roleUserCount.role_name}, Количество пользователей: {roleUserCount.user_count}");
            }
        }
        private static void RemoveUserFromRole(NpgsqlConnection connection, int userId, int roleId)
        {
            var sql = "DELETE FROM UserRoles WHERE user_id = @UserId AND role_id = @RoleId";
            connection.Execute(sql, new { UserId = userId, RoleId = roleId });
            Console.WriteLine("Пользователь успешно удален из роли.");
        }
        private static void ShowUsersAndRoles(NpgsqlConnection connection)
        {
            var userSql = "SELECT * FROM Users";
            var roleSql = "SELECT * FROM Roles";

            var users = connection.Query<User>(userSql).ToList();
            var roles = connection.Query<Role>(roleSql).ToList();

            Console.WriteLine("Users\tRoles");
            Console.WriteLine("-----------------------------");

            for (int i = 0; i < Math.Max(users.Count, roles.Count); i++)
            {
                string userString = i < users.Count ? $"ID: {users[i].user_id}, Username: {users[i].username}" : "";
                string roleString = i < roles.Count ? $"ID: {roles[i].role_id}, Role: {roles[i].role_name}" : "";

                Console.WriteLine($"{userString}\t{roleString}");
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Добавить пользователя");
            Console.WriteLine("2. Удалить пользователя");
            Console.WriteLine("3. Вывести информацию о пользователях");
            Console.WriteLine("4. Добавить новую роль");
            Console.WriteLine("5. Удалить роль");
            Console.WriteLine("6. Вывести список ролей");
            Console.WriteLine("7. Добавить пользователя в роль");
            Console.WriteLine("8. Показать пользователей с ролями");
            Console.WriteLine("9. Подсчитать количество пользователей в каждой роли");
            Console.WriteLine("10. Удалить пользователя из роли");
            Console.WriteLine("11. Показать пользователей и их роли");
            Console.WriteLine("12. Вызвать меню действий");
            Console.WriteLine("13. Выход");
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
