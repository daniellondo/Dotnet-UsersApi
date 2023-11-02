using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Api.Utils
{
    public class DataUtils
    {
        internal static void CreateTables(string _connectionString)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute($@" 
                    IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
                    BEGIN
                        CREATE TABLE Users (
                            Id INT PRIMARY KEY IDENTITY,
                            FirstName NVARCHAR(50),
                            LastName NVARCHAR(50),
                            Age INT NOT NULL,
                            BirthDate DATE NOT NULL,
                            Country NVARCHAR(100),
                            Province NVARCHAR(50),
                            City NVARCHAR(50)
                        );

                        CREATE INDEX IX_Users_Age ON Users (Age);
                        CREATE INDEX IX_Users_Country ON Users (Country);
                    END

                    IF OBJECT_ID(N'dbo.Login', N'U') IS NULL
                    BEGIN
                        CREATE TABLE Login (
                            Username NVARCHAR(50) PRIMARY KEY,
                            Password NVARCHAR(100) NOT NULL
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'UserTableType' AND is_table_type = 1)
                    BEGIN
                        CREATE TYPE dbo.UserTableType AS TABLE
                        (
                            FirstName NVARCHAR(50),
                            LastName NVARCHAR(50),
                            Country NVARCHAR(100),
                            Province NVARCHAR(50),
                            City NVARCHAR(50),
                            Age INT,
                            BirthDate DATE
                        );
                    END
                ");

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

    }
}
