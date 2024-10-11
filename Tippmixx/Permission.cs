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
using System.Xml.Linq;

namespace Tippmixx
{
    public class Role
    { 
        public static List<Role> All = new List<Role>();
        public int PermID { get; private set; }
        public string DisplayName { get; private set; }
        public int PermissibilityLevel { get; private set; }
        public PackIconKind RoleIcon { get; private set; }
        public Role(int permId, string displayName, int permissibilityLevel, string roleIcon)
        {
            PermID = permId;
            DisplayName = displayName;
            PermissibilityLevel = permissibilityLevel;
            RoleIcon = Misc.GetPackIconKindFromString(roleIcon);
            All.Add(this);
        }
        public static void BuildRoles() 
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                
                string query = @"
                SELECT PermID, DisplayName, PermissibilityLevel, RoleIconName 
                FROM PermSettings";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
        }
    }
    public class Permission : INotifyPropertyChanged
    {

        void OnPropertyChanged([CallerMemberName] string name = null) 
        {

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            DataHandler.PermissionUpdate(this, name);
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
