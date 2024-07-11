using Dapper;
using Npgsql;
using System;
using System.Diagnostics;
using System.IO;
using TestPostgres;

namespace TestPostgres
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Terminal PostrgesSQL TestTimeManagement db";

            string connectionString = GetConnectionString();

            // Создание объекта класса PostgreSQLDatabaseManager
            PostgreSQLDatabaseManager databaseManager = new PostgreSQLDatabaseManager(connectionString);

            using (var connection = databaseManager.GetConnection())
            {
                Console.WriteLine("Добро пожаловать в консольное приложение по управлению базой данных TestTimeManagement!");

                PostgreSQLDatabaseManager.ShowMenu();

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
                            PostgreSQLDatabaseManager.AddUser(connection, username);
                            break;
                        case "2":
                            Console.Write("Введите ID пользователя для удаления: ");
                            var userIdToDelete = Convert.ToInt32(Console.ReadLine());
                            PostgreSQLDatabaseManager.DeleteUser(connection, userIdToDelete);
                            break;
                        case "3":
                            PostgreSQLDatabaseManager.ShowUsers(connection);
                            break;
                        case "4":
                            Console.Write("Введите имя роли: ");
                            var roleName = Console.ReadLine();
                            PostgreSQLDatabaseManager.AddRole(connection, roleName);
                            break;
                        case "5":
                            Console.Write("Введите ID роли для удаления: ");
                            var roleIdToDelete = Convert.ToInt32(Console.ReadLine());
                            PostgreSQLDatabaseManager.DeleteRole(connection, roleIdToDelete);
                            break;
                        case "6":
                            PostgreSQLDatabaseManager.ShowRoles(connection);
                            break;
                        case "7":
                            Console.Write("Введите ID пользователя: ");
                            var userId = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Введите ID роли: ");
                            var roleId = Convert.ToInt32(Console.ReadLine());
                            PostgreSQLDatabaseManager.AddUserToRole(connection, userId, roleId);
                            break;
                        case "8":
                            PostgreSQLDatabaseManager.ShowUsersWithRoles(connection);
                            break;
                        case "9":
                            PostgreSQLDatabaseManager.CountUsersInRoles(connection);
                            break;
                        case "10":
                            Console.Write("Введите ID пользователя: ");
                            var deleteUserId = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Введите ID роли: ");
                            var deleteRoleId = Convert.ToInt32(Console.ReadLine());
                            PostgreSQLDatabaseManager.RemoveUserFromRole(connection, deleteUserId, deleteRoleId);
                            break;
                        case "11":
                            PostgreSQLDatabaseManager.ShowUsersAndRoles(connection);
                            break;
                        case "12":
                            PostgreSQLDatabaseManager.ShowMenu();
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

            // Метод для получения строки подключения из конфигурационного файла
            string GetConnectionString()
            {
                // Путь к файлу конфигурации
                var configFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

                if (!File.Exists(configFilePath))
                {
                    throw new FileNotFoundException($"Файл конфигурации appsettings.json не найден по пути: {configFilePath}");
                }

                // Читаем файл конфигурации
                var config = File.ReadAllText(configFilePath);

                // Парсим JSON
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(config);

                // Получаем строку подключения
                return jsonObject.ConnectionStrings.TestTimeManagement;
            }
        }
    }
}



