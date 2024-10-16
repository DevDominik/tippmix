﻿using Google.Protobuf.WellKnownTypes;
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
    public class Organization
    {
        string displayName;
        int id;
        int balance;
        public Organization(int id, string displayName, int balance)
        {
            this.id = id;
            this.displayName = displayName;
            this.balance = balance;
        }
        public string DisplayName { get { return displayName; } set { displayName = value; } }
        public int OrgID { get { return id; } }
        public int Balance { get { return balance; } set { balance = value; } }
    }
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

        void OnPropertyChanged([CallerMemberName] string name = null)
        {
            DataHandler.UpdateBettorData(this, name);
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
        }
        void UpdateAccessStatus()
        {
            UserStatusAsIcon = isActive ? PackIconKind.AccountCheck : PackIconKind.AccountCancel;
            AllowAccessAsIcon = !isActive ? PackIconKind.Tick : PackIconKind.Ban;
            AllowAccessAsString = !isActive ? "Reactivate" : "Deactivate";
        }
        public Permission HighestPermission()
        {
            Permission toReturn = null;
            foreach (Permission permission in Permissions.Where(x => x.IsActive))
            {
                if (toReturn == null)
                {
                    toReturn = permission;
                }
                else
                {
                    if (toReturn.Role.PermissibilityLevel < permission.Role.PermissibilityLevel)
                    {
                        toReturn = permission;
                    }
                }
            }
            return toReturn;
        }

        public bool HasPermissibilityLevel(int level)
        {
            return Permissions.Any(x => x.Role.PermissibilityLevel == level && x.IsActive);
        }
        public bool HasPermissibilityLevel(int[] levels)
        {
            ObservableCollection<Permission> toWorkWith = Permissions;
            foreach (int level in levels)
            {
                if (toWorkWith.Any(x => x.Role.PermissibilityLevel == level && x.IsActive))
                {
                    return true;
                }
            }
            return false;
        }
        public int Id { get { return id; } }
        public string Username { get { return username; } set { username = value; OnPropertyChanged(); } }
        public int Balance { get { return balance; } set { balance = value; OnPropertyChanged(); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged(); } }
        public string Password { get { return password; } set { password = value; OnPropertyChanged(); } }
        public DateTime JoinDate { get { return joindate; } set { joindate = value; OnPropertyChanged(); } }
        public bool IsActive { get { return isActive; } set { isActive = value; OnPropertyChanged(); UpdateAccessStatus(); } }
        public PackIconKind UserStatusAsIcon { get; private set; }
        public PackIconKind AllowAccessAsIcon {  get; private set; }
        public string AllowAccessAsString { get; private set; }
        public ObservableCollection<Permission> Permissions { get { return DataHandler.GetUserPermissions(this); } }
    }
}
