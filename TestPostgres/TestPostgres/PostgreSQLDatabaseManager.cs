using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPostgres
{
    internal class PostgreSQLDatabaseManager
    {
        private readonly NpgsqlConnection _connection;

        // Конструктор класса, инициализирует подключение
        public PostgreSQLDatabaseManager(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        // Метод для получения открытого подключения
        public NpgsqlConnection GetConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        // Метод для закрытия подключения
        public void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public static void AddUser(NpgsqlConnection connection, string username)
        {
            var sql = "INSERT INTO Users (user_id, username) VALUES (nextval('user_id_seq'), @Username)";
            connection.Execute(sql, new { Username = username });
            Console.WriteLine("Пользователь успешно добавлен.");
        }

        public static void DeleteUser(NpgsqlConnection connection, int userId)
        {
            var sql = "DELETE FROM Users WHERE user_id = @UserId";
            connection.Execute(sql, new { UserId = userId });
            Console.WriteLine("Пользователь успешно удален.");
        }

        public static void ShowUsers(NpgsqlConnection connection)
        {
            var sql = "SELECT * FROM Users";
            var users = connection.Query<User>(sql);
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.user_id}, Username: {user.username}");
            }
        }

        public static void AddRole(NpgsqlConnection connection, string role_name)
        {
            var sql = "INSERT INTO Roles (role_id, role_name) VALUES (nextval('user_id_seq'), @Role)";
            connection.Execute(sql, new { Role = role_name });
            Console.WriteLine("Новая роль успешно создана!");
        }

        public static void DeleteRole(NpgsqlConnection connection, int role_id)
        {
            var sql = "DELETE FROM Roles WHERE role_id = @RoleId";
            connection.Execute(sql, new { RoleId = role_id });
            Console.WriteLine("Роль удалена.");
        }

        public static void ShowRoles(NpgsqlConnection connection)
        {
            var sql = "SELECT * FROM Roles";
            var roles = connection.Query<Role>(sql);
            foreach (var role in roles)
            {
                Console.WriteLine($"ID: {role.role_id}, Role: {role.role_name}");
            }
        }
        public static void AddUserToRole(NpgsqlConnection connection, int userId, int roleId)
        {
            var sql = "INSERT INTO UserRoles (user_id, role_id) VALUES (@UserId, @RoleId)";
            connection.Execute(sql, new { UserId = userId, RoleId = roleId });
            Console.WriteLine("Пользователь успешно добавлен к роли.");
        }
        public static void ShowUsersWithRoles(NpgsqlConnection connection)
        {
            var sql = "SELECT Users.username, Roles.role_name FROM Users " +
                      "JOIN UserRoles ON Users.user_id = UserRoles.user_id " +
                      "JOIN Roles ON UserRoles.role_id = Roles.role_id";
            var userRoles = connection.Query(sql);
            Console.WriteLine("Users\tRoles");
            Console.WriteLine("-----------------------------");
            foreach (var userRole in userRoles)
            {
                Console.WriteLine($"Username: {userRole.username}\t Role: {userRole.role_name}");
            }
        }
        public static void CountUsersInRoles(NpgsqlConnection connection)
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
        public static void RemoveUserFromRole(NpgsqlConnection connection, int userId, int roleId)
        {
            var sql = "DELETE FROM UserRoles WHERE user_id = @UserId AND role_id = @RoleId";
            connection.Execute(sql, new { UserId = userId, RoleId = roleId });
            Console.WriteLine("Пользователь успешно удален из роли.");
        }
        public static void ShowUsersAndRoles(NpgsqlConnection connection)
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

        public static void ShowMenu()
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
            Console.WriteLine("11. Показать пользователей и роли");
            Console.WriteLine("12. Вызвать меню действий");
            Console.WriteLine("13. Выход");
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
