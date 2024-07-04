 -- Создание таблицы Пользователи
CREATE TABLE Users (
  user_id INT PRIMARY KEY,
  username VARCHAR(255) NOT NULL
);

-- Создание таблицы Роли
CREATE TABLE Roles (
  role_id INT PRIMARY KEY,
  role_name VARCHAR(255) NOT NULL
);

-- Создание таблицы ПользовательРоль
CREATE TABLE UserRoles (
  userroles_id INT PRIMARY KEY,
  user_id INT,
  role_id INT,
  FOREIGN KEY (user_id) REFERENCES Users(user_id),
  FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

CREATE SEQUENCE user_id_seq;