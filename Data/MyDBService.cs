using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using MyProject.Components.Pages;
using MyProject.Models;

namespace MyProject.Data
{
    public class MyDBService
    {
        private readonly string _connectionString = "Data Source=Data/MyDB.db;Version=3;";


        // קריאה של כל המשתמשים
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand("SELECT Id, FullName, Email, Password, Role FROM Users", connection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                User user = new User();
                user.SetId(reader.GetInt32(0));
                user.SetFullName(reader.GetString(1));
                user.SetEmail(reader.GetString(2));
                user.SetPassword(reader.GetString(3));
                user.SetRole(reader.IsDBNull(4) ? null : reader.GetString(4));
                users.Add(user);
            }

            reader.Close();
            connection.Close();

            return users;
        }

        // הוספת משתמש חדש
        public void AddUser(string fullName, string email, string password, string role)
        {
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(
                "INSERT INTO Users (FullName, Email, Password, Role) VALUES (@FullName, @Email, @Password, @Role)",
                connection);
            command.Parameters.AddWithValue("@FullName", fullName);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@Role", role);

            command.ExecuteNonQuery();
            connection.Close();
        }

        // מחיקת משתמש לפי מזהה
        public void DeleteUser(int id)
        {
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand("DELETE FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            connection.Close();
        }

        // עדכון פרטי משתמש לפי מזהה
        public void UpdateUser(int id, string fullName, string email, string password, string role)
        {
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(
                "UPDATE Users SET FullName = @FullName, Email = @Email, Password = @Password, Role = @Role WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@FullName", fullName);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@Role", role);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            connection.Close();
        }

        public User? AuthenticateUser(string email, string password)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.GetEmail() == email && u.GetPassword() == password);
        }

    }
}
