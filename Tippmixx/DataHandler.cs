using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Tippmixx
{
    public class DataHandler
    {
        static DataHandler instance;
        static MySqlConnection connection;
        DataHandler(string connectionString) 
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }
        ~DataHandler() {
            connection.Close();
        }
        
        
        public static void Initialize()
        {
            if (instance != null) { throw new TaskSchedulerException("There's already a DataHandler instance running in the background."); }
            instance = new("Server=localhost;Database=tippmix;User ID=root;Password=;");
        }

        /* 
         * A
         * |
         * |
         * | Automation, don't touch it
         * 
         */

        public static void GetBettorData(User user, string[] propNames)
        {
            string query = $"SELECT {string.Join(",", propNames)} FROM `bettors` WHERE `bettors`.`BettorsID` = @bettorId;";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    cmd.Parameters.AddWithValue("@bettorId", user.Id);
                    if (reader.Read())
                    {
                        foreach (string property in propNames)
                        {
                            user.GetType().GetProperty(property).SetValue(user, reader[property]);
                        }
                    }
                }
            }
        }

        public static ObservableCollection<User> GetAllBettors(string input = "-1")
        {
            ObservableCollection<User> UsersList = new();
            string query;
            if (string.IsNullOrWhiteSpace(input))
            {
                query = @"
    SELECT BettorsID, Username, Email, Password, JoinDate, Balance, IsActive 
    FROM Bettors";
            }
            else if (int.TryParse(input, out int parsedId))
            {
                query = $"SELECT BettorsID, Username, Password, Email, JoinDate, Balance, IsActive FROM Bettors WHERE BettorsID LIKE '{parsedId}%'";
            }
            else if (input.Any(char.IsLetter))
            {
                query = $"SELECT BettorsID, Username, Email, Password, JoinDate, Balance, IsActive FROM Bettors WHERE Username LIKE '{input}%'";
            }
            else
            {
                query = @"
    SELECT BettorsID, Username, Email, Password, JoinDate, Balance, IsActive 
    FROM Bettors";
            }

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UsersList.Add(new User(
                            Convert.ToInt32(reader["BettorsID"]),
                            reader["Username"].ToString(),
                            reader["Password"].ToString(),
                            Convert.ToInt32(reader["Balance"]),
                            reader["Email"].ToString(),
                            DateTime.Parse(reader["JoinDate"].ToString()),
                            (bool)reader["IsActive"]
                        ));
                    }
                }
            }
            foreach (User user in UsersList)
            {
                user.Permissions = GetUserPermissions(user);
            }
            return UsersList;
        }

        public static void UpdateBettorData(User user, string name) 
        {
            string updateQueue = user.GetType().GetProperty(name).GetValue(user, null).ToString();
            if (name == "IsActive")
            {
                updateQueue = user.IsActive ? "1" : "0";
            }
            string query = $"UPDATE `bettors` SET `{name}` = '{updateQueue}' WHERE `bettors`.`BettorsID` = @bettorId";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@bettorId", user.Id);
            }
        }

        public static void BuildRoles()
        {
            string query = @"
                SELECT PermID, DisplayName, PermissibilityLevel, RoleIconName 
                FROM PermSettings";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        new Role(Convert.ToInt32(reader["PermID"]), reader["DisplayName"].ToString(), Convert.ToInt32(reader["PermissibilityLevel"]), reader["RoleIconName"].ToString());
                    }
                }
            }
        }
        public static Permission? HighestPermission(User user)
        {
            return user.Permissions
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Role.PermissibilityLevel)
                .FirstOrDefault();
        }
        public static ObservableCollection<Permission> GetUserPermissions(User user)
        {
            ObservableCollection<Permission> permissions = new ObservableCollection<Permission>();
            string query = @$"
        SELECT ID, PermID, IsActive 
        FROM Perms 
        WHERE BettorID = {user.Id}";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Role role = Role.All.FirstOrDefault(x => x.PermID == Convert.ToInt32(reader["PermID"]));

                        if (role != null)
                        {
                            Permission permission = new Permission(
                                Convert.ToInt32(reader["ID"]),
                                user.Id,
                                (bool)reader["IsActive"],
                                role
                            );
                            permissions.Add(permission);
                        }
                    }
                }
            }
            return permissions;
        }

        public static void PermissionUpdate(Permission permission, string name)
        {
            string updateQueue = permission.GetType().GetProperty(name).GetValue(permission, null).ToString();
            if (name == "IsActive")
            {
                updateQueue = permission.IsActive ? "1" : "0";
            }
            string query = $"UPDATE `perms` SET `{name}` = '{updateQueue}' WHERE `perms`.`ID` = {permission.ID}";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

       
    }
}
