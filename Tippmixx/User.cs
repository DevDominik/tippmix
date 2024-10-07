using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tippmixx
{
    internal class User : INotifyPropertyChanged
    {
        int id;
        string username;
        string password;
        int balance;
        string email;
        DateTime joindate;
        bool isActive;


        public event PropertyChangedEventHandler PropertyChanged;

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
                            UsersList.Add(new User(Convert.ToInt32(reader["BettorsID"]),reader["Username"].ToString(), Convert.ToInt32(reader["Balance"]), reader["Email"].ToString(), DateTime.Parse(reader["JoinDate"].ToString()), (bool)reader["IsActive"]));
                        }
                    }
                }
            }
            return UsersList;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = $"UPDATE `bettors` SET `{name}` = '{GetType().GetProperty(name).GetValue(this, null)}' WHERE `bettors`.`BettorsID` = {id}";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public User(int id, string username, int balance, string email, DateTime joindate, bool isActive)
        {
            this.id = id;
            this.username = username;
            this.balance = balance;
            this.email = email;
            this.joindate = joindate;
            this.isActive = isActive;
        }

        public int Id { get { return id; } set { id = value; OnPropertyChanged(); } }
        public string Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public int Balance { get { return balance; } set {  balance = value; OnPropertyChanged(); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged(); } }
                
        public DateTime JoinDate { get { return joindate; } set { joindate = value; OnPropertyChanged(); } }
        public bool IsActive { get { return isActive; } set { isActive = value; OnPropertyChanged(); } }
    }
}
