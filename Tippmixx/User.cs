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
    public class User : INotifyPropertyChanged
    {
        public static User Session { get; set; }
        int id;
        string username;
        string password;
        int balance;
        string email;
        DateTime joindate;
        bool isActive;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (!IsAllowedToLiveUpdate)
            {
                return;
            }
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            DataHandler.UpdateBettorData(this, name);
        }
        public User()
        {
            IsAllowedToLiveUpdate = false;
        }
        public User(int id, string username, string password, int balance, string email, DateTime joindate, bool isActive)
        {
            this.id = id;
            this.username = username;
            this.balance = balance;
            this.email = email;
            this.joindate = joindate;
            this.isActive = isActive;
            this.password = password;
            UpdateAccessStatus();
            IsAllowedToLiveUpdate = true;
        }
        void UpdateAccessStatus()
        {
            UserStatusAsIcon = isActive ? PackIconKind.AccountCheck : PackIconKind.AccountCancel;
            AllowAccessAsIcon = !isActive ? PackIconKind.Tick : PackIconKind.Ban;
            AllowAccessAsString = !isActive ? "Reactivate" : "Deactivate";
        }

        public bool HasPermissibilityLevel(int level)
        {
            return Permissions.Any(x => x.Role.PermissibilityLevel == level && x.IsActive);
        }

        public int Id { get { return id; } set { id = value; OnPropertyChanged(); } }
        public string Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public int Balance { get { return balance; } set {  balance = value; OnPropertyChanged(); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged(); } }
        public string Password { get { return password; } set { password = value; OnPropertyChanged(); } }
        public DateTime JoinDate { get { return joindate; } set { joindate = value; OnPropertyChanged(); } }
        public bool IsActive { get { return isActive; } set { isActive = value; OnPropertyChanged(); UpdateAccessStatus(); } }
        public bool IsAllowedToLiveUpdate { get; set; }
        public PackIconKind UserStatusAsIcon { get; private set; }
        public PackIconKind AllowAccessAsIcon {  get; private set; }
        public string AllowAccessAsString { get; private set; }
        public ObservableCollection<Permission> Permissions { get { return DataHandler.GetUserPermissions(this); } }
    }
}
