using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tippmixx
{
    internal class DataBase
    {
        public static ObservableCollection<User> RefreshUserList()
        {
            ObservableCollection<User> UsersList = new();
            UsersList.Clear();
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                SELECT BettorsID, Username, Email, JoinDate, Balance, IsActive 
                FROM Bettors 
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UsersList.Add(new User(Convert.ToInt32(reader["BettorsID"]), reader["Username"].ToString(), Convert.ToInt32(reader["Balance"]), reader["Email"].ToString(), DateTime.Parse(reader["JoinDate"].ToString()), (bool)reader["IsActive"]));
                        }
                    }
                }
            }
            return UsersList;
        }


        public static bool AuthenticateUser(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                SELECT Username, Email, Balance, IsActive 
                FROM Bettors 
                WHERE Username = @username AND Password = @password";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username.ToLower());
                    cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));

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

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

        }
    }
}
