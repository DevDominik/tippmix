using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tippmixx
{
    internal class Users
    {
        int id;
        string username;
        string password;
        int balance;
        string email;
        DateTime joindate;
        bool status;

        public static List<Users> RefreshUserList()
        {
            List<Users> UsersList = new();
            UsersList.Clear();
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                SELECT BettorsID, Username, Password, Email, JoinDate, Balance, IsActive 
                FROM Bettors 
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UsersList.Add(new Users(Convert.ToInt32(reader["BettorsID"]),reader["Username"].ToString(), reader["Password"].ToString(), Convert.ToInt32(reader["Balance"]), reader["Email"].ToString(), DateTime.Parse(reader["JoinDate"].ToString()), (bool)reader["IsActive"]));
                        }
                    }
                }
            }
            return UsersList;
        }


        public Users(int id, string username, string password, int balance, string email, DateTime joindate, bool status)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
            this.Balance = balance;
            this.Email = email;
            this.Joindate = joindate;
            this.Status = status;
        }

        public int Id { get => id; set => id = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public int Balance { get => balance; set => balance = value; }
        public string Email { get => email; set => email = value; }
        public DateTime Joindate { get => joindate; set => joindate = value; }
        public bool Status { get => status; set => status = value; }
    }
}
