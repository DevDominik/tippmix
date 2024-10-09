using Google.Protobuf.WellKnownTypes;
using MaterialDesignThemes.Wpf;
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

        public static ObservableCollection<User> RefreshUserList(string id = "-1")
        {
            ObservableCollection<User> UsersList = new();
            UsersList.Clear();

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();

                string query;

                // Check if the id is -1, invalid, or empty
                if (id == "-1" || string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int parsedId))
                {
                    // If id is -1 or invalid/empty input, return all users
                    query = @"
            SELECT BettorsID, Username, Email, JoinDate, Balance, IsActive 
            FROM Bettors";
                }
                else
                {
                    // Valid ID, fetch the specific user
                    query = $"SELECT BettorsID, Username, Email, JoinDate, Balance, IsActive FROM Bettors WHERE BettorsID = {parsedId}";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UsersList.Add(new User(
                                Convert.ToInt32(reader["BettorsID"]),
                                reader["Username"].ToString(),
                                Convert.ToInt32(reader["Balance"]),
                                reader["Email"].ToString(),
                                DateTime.Parse(reader["JoinDate"].ToString()),
                                (bool)reader["IsActive"]
                            ));
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
                string updateQueue = GetType().GetProperty(name).GetValue(this, null).ToString();
                if (name == "IsActive") 
                { 
                    updateQueue = isActive ? "1" : "0";
                }
                string query = $"UPDATE `bettors` SET `{name}` = '{updateQueue}' WHERE `bettors`.`BettorsID` = {id}";

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
            UpdateAccessStatus();
        }
        void UpdateAccessStatus()
        {
            UserStatusAsIcon = isActive ? PackIconKind.AccountCheck : PackIconKind.AccountCancel;
            AllowAccessAsIcon = !isActive ? PackIconKind.Tick : PackIconKind.Ban;
            AllowAccessAsString = !isActive ? "Reactivate" : "Deactivate";
        }

        public int Id { get { return id; } set { id = value; OnPropertyChanged(); } }
        public string Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public int Balance { get { return balance; } set {  balance = value; OnPropertyChanged(); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged(); } }
                
        public DateTime JoinDate { get { return joindate; } set { joindate = value; OnPropertyChanged(); } }
        public bool IsActive { get { return isActive; } set { isActive = value; OnPropertyChanged(); UpdateAccessStatus(); } }
        public PackIconKind UserStatusAsIcon { get; private set; }
        public PackIconKind AllowAccessAsIcon {  get; private set; }
        public string AllowAccessAsString { get; private set; }
    }
}
