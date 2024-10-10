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
using System.Xml.Linq;

namespace Tippmixx
{
    public class Role
    {
        public static List<Role> All = new List<Role>();
        public int PermID { get; }
        public string DisplayName { get; }
        public int PermissibilityLevel { get; }
        public Role(int permId, string displayName, int permissibilityLevel) 
        {
            PermID = permId;
            DisplayName = displayName;
            PermissibilityLevel = permissibilityLevel;
            All.Add(this);
        }
        public static void BuildRoles() 
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();

                string query = @"
                SELECT PermID, DisplayName, PermissibilityLevel 
                FROM PermSettings";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            new Role(Convert.ToInt32(reader["PermID"]), reader["DisplayName"].ToString(), Convert.ToInt32(reader["PermissibilityLevel"]));
                        }
                    }
                }
            }
        }
    }
    public class Permission : INotifyPropertyChanged
    {
        public static bool HasPermissibilityLevel(User user, int level)
        {
            return user.Permissions.Any(x => x.Role.PermissibilityLevel == level && x.IsActive);
        }
        public static ObservableCollection<Permission> GetUserPermissions(int id)
        {
            ObservableCollection<Permission> permissions = new ObservableCollection<Permission>();
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @$"
        SELECT ID, PermID, IsActive 
        FROM Perms 
        WHERE BettorID = {id}";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Role role = Tippmixx.Role.All.FirstOrDefault(x => x.PermID == Convert.ToInt32(reader["PermID"]));

                            if (role != null)
                            {
                                Permission permission = new Permission(
                                    Convert.ToInt32(reader["ID"]),
                                    id,
                                    (bool)reader["IsActive"],
                                    role
                                );
                                permissions.Add(permission);
                                MessageBox.Show($"Permission added: {permission.Role.PermID}, IsActive: {permission.IsActive}");
                            }
                            else
                            {
                                MessageBox.Show($"No matching role found for PermID: {reader["PermID"]}");
                            }
                        }
                    }
                }
            }
            return permissions;
        }

        void OnPropertyChanged([CallerMemberName] string name = null) 
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
                string query = $"UPDATE `perms` SET `{name}` = '{updateQueue}' WHERE `perms`.`ID` = {id}";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        int id;
        int bettorID;
        bool isActive;
        Role role;
        public Permission(int id, int bettorID, bool isActive, Role role) 
        { 
            this.id = id;
            this.bettorID = bettorID;
            this.isActive = isActive;
            this.role = role;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public int ID { get { return id; } }
        public bool IsActive { get { return isActive; } set { isActive = value; OnPropertyChanged(); } }
        public int BettorID { get { return bettorID; } }
        public Role Role { get { return role; } }
    }
}
