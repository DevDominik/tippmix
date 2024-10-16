﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

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
            BuildRoles();
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

        public static ObservableCollection<User> GetAllBettors(string input = null)
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
            User match = UsersList.FirstOrDefault(x => x.Id == User.Session.Id);
            if (match != null)
            {
                UsersList.Remove(match);
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
                cmd.ExecuteNonQuery();
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


        public static ObservableCollection<Bet> GetUserBets(User user)
        {
            ObservableCollection<Event> events = RefreshEventList(null);
            ObservableCollection<Bet> bets = new ObservableCollection<Bet>();
            string query = "SELECT Events.EventName, Bets.Amount, Bets.Odds, Bets.BetDate " +
                                  "FROM Bets " +
                                  "WHERE Bets.BettorsID = @BettorsID";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@BettorsID", user.Id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        Event assignedEvent = events.FirstOrDefault(x => x.EventID == Convert.ToInt32(reader["EventID"]));

                        if (assignedEvent != null)
                        {
                            Bet bet = new Bet(
                                Convert.ToInt32(reader["BetsID"]),
                                Convert.ToDateTime(reader["BetDate"]),
                                float.Parse(reader["Odds"].ToString()),
                                Convert.ToInt32(reader["Amount"]),
                                Convert.ToInt32(reader["BettorsID"]),
                                Convert.ToBoolean(reader["Status"]),
                                assignedEvent
                            );
                            bets.Add(bet);
                        }
                    }
                }
            }
            return bets;
        }



        public static void CreateEvent(string eventName, DateTime eventDate, string category, string location)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                INSERT INTO Events (EventName, EventDate, Category, Location) 
                VALUES (@EventName, @EventDate, @Category, @Location);
                SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EventName", eventName);
                    cmd.Parameters.AddWithValue("@EventDate", eventDate);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Location", location);

                    int newEventID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public static ObservableCollection<Organization> GetAllOrganizations() 
        { 
            ObservableCollection<Organization> organizations = new ObservableCollection<Organization>();

            return organizations;
        }
        public static Organization GetOrganization(int id) 
        {
            return GetAllOrganizations().Where(x => x.OrgID == id).FirstOrDefault();
        }
        public static ObservableCollection<Event> GetLinkedEvents(Organization organization) 
        {
            ObservableCollection<Event> events = new ObservableCollection<Event>();
            return events;
        }
        public static ObservableCollection<Event> RefreshEventList(string input = null)
        {
            ObservableCollection<Event> eventList = new();
            string query;

            if (string.IsNullOrWhiteSpace(input))
            {
                query = @"
                    SELECT EventID, EventName, EventDate, Category, Location 
                    FROM Events";
            }
            else if (int.TryParse(input, out int parsedId))
            {
                query = $"SELECT EventID, EventName, EventDate, Category, Location FROM Events WHERE EventID LIKE '{parsedId}%'";
            }
            else if (input.Any(char.IsLetter))
            {
                query = $"SELECT EventID, EventName, EventDate, Category, Location FROM Events WHERE EventName LIKE '{input}%'";
            }
            else
            {
                query = @"
                    SELECT EventID, EventName, EventDate, Category, Location 
                    FROM Events";
            }
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        eventList.Add(new Event(
                            Convert.ToInt32(reader["EventID"]),
                            reader["EventName"].ToString(),
                            DateTime.Parse(reader["EventDate"].ToString()),
                            reader["Category"].ToString(),
                            reader["Location"].ToString()
                        ));
                    }
                }
            }
            ObservableCollection<Organization> organizationList = GetAllOrganizations();

            return eventList;
        }


        public static bool Login(string username, string password)
        {
            string query = @"
                SELECT Username, Email, Balance, IsActive, BettorsID, JoinDate 
                FROM Bettors 
                WHERE Username = @username AND Password = @password";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username.ToLower());
                cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User session = new User(Convert.ToInt32(reader["BettorsID"]), reader["Username"].ToString(), EasyEncryption.SHA.ComputeSHA256Hash(password), Convert.ToInt32(reader["Balance"]), reader["Email"].ToString(), Convert.ToDateTime(reader["JoinDate"]), Convert.ToBoolean(reader["IsActive"]));
                        User.Session = session;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }



        public static bool Register(string username, string password, string email, int balance)
        {
            string checkQuery = "SELECT COUNT(1) FROM Bettors WHERE Username = @username OR Email = @Email";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@username", username.ToLower());
                checkCmd.Parameters.AddWithValue("@Email", email.ToLower());

                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists > 0)
                {
                    return false;
                }
            }

            string insertQuery = @"
            INSERT INTO Bettors (Username, Balance, Email, Password, JoinDate, IsActive)
            VALUES (@username, @balance, @Email, @password, @joinDate, @isActive)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@username", username.ToLower());
                cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));
                cmd.Parameters.AddWithValue("@Email", email.ToLower());
                cmd.Parameters.AddWithValue("@balance", balance);
                cmd.Parameters.AddWithValue("@joinDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@isActive", true);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    //MessageBox.Show("Registration successful.", "Auth", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    //MessageBox.Show("Registration failed. Please try again.", "Auth", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
        }


        private bool AuthenticateUser(string username, string password)
        {
            string query = @"
                SELECT Username, Email, Balance, IsActive, BettorsID, JoinDate 
                FROM Bettors 
                WHERE Username = @username AND Password = @password";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username.ToLower());
                cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User session = new User(Convert.ToInt32(reader["BettorsID"]), reader["Username"].ToString(), EasyEncryption.SHA.ComputeSHA256Hash(password), Convert.ToInt32(reader["Balance"]), reader["Email"].ToString(), Convert.ToDateTime(reader["JoinDate"]), Convert.ToBoolean(reader["IsActive"]));
                        User.Session = session;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }


        public static void PlaceBet(User user, int eventId, float odds, int amount)
        {
            string query = @"
                INSERT INTO Bets (BetDate, Odds, Amount, BettorsID, EventID, Status) 
                VALUES (@BetDate, @Odds, @Amount, @BettorsID, @EventID, @Status);";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@BetDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Odds", odds);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@BettorsID", user.Id);
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@Status", true); // Assuming the status is true when placing a bet

                cmd.ExecuteNonQuery(); // Execute the command to insert the bet
                Console.WriteLine("Bet placed successfully for EventID: " + eventId);
            }
        }

        public static void UpdatePermission(Permission permission, string name)
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

        public static string AddKey(Role role) 
        {
            string key = Misc.RandomString(12);
            string query = @"
                INSERT INTO `rolekey` (KeyID, ForRole, IsActive) 
                VALUES (@key, @role, @isactive);";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                cmd.Parameters.AddWithValue("@role", role.PermID);
                cmd.Parameters.AddWithValue("@isactive", "1");
                cmd.ExecuteNonQuery();
            }
            return key;
        }

        public static bool DeleteKey(string key) 
        {
            string query = "SELECT IsActive FROM `rolekey` WHERE `rolekey`.`KeyID` = @key";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool isActive = Convert.ToBoolean(reader["IsActive"].ToString());
                        if (!isActive) { return false; }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            query = $"UPDATE `rolekey` SET `IsActive` = '0' WHERE `rolekey`.`KeyID` = @key";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                cmd.ExecuteNonQuery();
            }
            return true;
        }
        public static bool UseRoleKey(User user, string key)
        {
            string query = "SELECT ForRole, IsActive FROM `rolekey` WHERE `rolekey`.`KeyID` = @key";
            int roleId = 0;
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool isActive = Convert.ToBoolean(reader["IsActive"].ToString());
                        roleId = Convert.ToInt32(reader["ForRole"].ToString());
                        if (!isActive) { return false; }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            query = $"UPDATE `rolekey` SET `IsActive` = '0' WHERE `rolekey`.`KeyID` = @key";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                cmd.ExecuteNonQuery();
            }
            user.Permissions.Where(x => x.Role.PermID == roleId).First().IsActive = true;
            return true;
        }
    }
}
