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
    user_id INT REFERENCES Users(user_id),
    role_id INT REFERENCES Roles(role_id),
    PRIMARY KEY (user_id, role_id)
);

CREATE SEQUENCE user_id_seq