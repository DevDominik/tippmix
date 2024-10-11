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
                user.Permissions = Permission.GetUserPermissions(user);
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

        //public static bool RequestWithoutQuery(string command) 
        //{
        //    return true;
        //}
        //public static List<T> RequestWithQuery<T>(string command, Dictionary<string, string> argumentDefiner, Dictionary<string, Type> typeDefiner) 
        //{
        //    List<T> toReturn = new List<T>();
        //    int index = 0;
        //    Type[] typeArray = new Type[typeDefiner.Count];
        //    foreach (KeyValuePair<string, Type> kvp in typeDefiner)
        //    {
        //        typeArray[index] = kvp.Value;
        //        index++;
        //    }
        //    ConstructorInfo ctor = typeof(T).GetConstructor(typeArray);
        //    ParameterInfo[] parameters = ctor.GetParameters();
        //    foreach (KeyValuePair<string, string> kvp in argumentDefiner)
        //    {

        //    }
        //    using (MySqlCommand cmd = new MySqlCommand(command, connection))
        //    {
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {

        //            while (reader.Read())
        //            {

        //                T final = (T)Activator.CreateInstance(typeof(T), parameters);
        //                toReturn.Add(final);
        //            }
        //        }
        //    }
        //    return toReturn;
        //}
    }
}
