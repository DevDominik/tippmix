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

        static List<Users> UsersList = new();


        int id;
        string username;
        string password;
        int balance;
        string email;
        DateTime joindate;
        bool status;

        private void RefreshUserList()
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                SELECT Username, Email, Balance, IsActive 
                FROM Bettors 
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session.Username = reader["Username"].ToString();
                            Session.Password = EasyEncryption.SHA.ComputeSHA256Hash(password);
                            Session.Email = reader["Email"].ToString();
                            Session.Balance = Convert.ToInt32(reader["Balance"]);
                            Session.IsActive = Convert.ToBoolean(reader["IsActive"]);
                            //Session.Perm.Add();
                        }
                    }
                }
            }
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
