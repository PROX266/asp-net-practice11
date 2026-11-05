USE webstore;
GO

-- Столбец для хеша PasswordHasher<User>
IF COL_LENGTH('dbo.users', 'password_hash') IS NULL
    ALTER TABLE dbo.users ADD password_hash NVARCHAR(500) NULL;
GO

-- Роли: Admin, Manager, User
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE role_name = N'Admin')
    INSERT INTO dbo.roles (role_name) VALUES (N'Admin');
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE role_name = N'Manager')
    INSERT INTO dbo.roles (role_name) VALUES (N'Manager');
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE role_name = N'User')
    INSERT INTO dbo.roles (role_name) VALUES (N'User');
GO

SELECT ID, role_name FROM dbo.roles;
SELECT ID, user_login, is_active, role_ID, password_hash FROM dbo.users;
GO
