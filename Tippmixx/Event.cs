using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tippmixx
{
    using System;
    using System.Collections.ObjectModel;
    using MaterialDesignThemes.Wpf;
    using MySql.Data.MySqlClient;

    public class EventManager
    {
    }

    public class Event
    {
        int eventId;
        string eventName;
        DateTime eventDate;
        string category;
        string location;
        bool isActive;
        Organization belongsToOrg;
        public Event(int eventId, string eventName, DateTime eventDate, string category, string location) 
        {
            this.eventId = eventId;
            this.eventName = eventName;
            this.eventDate = eventDate;
            this.category = category;
            this.location = location;
        }
        void UpdateAccessStatus()
        {
            AllowBettingAsIcon = !isActive ? PackIconKind.Tick : PackIconKind.Ban;
            AllowBettingAsString = !isActive ? "Allow" : "Halt";
        }
        public int EventID { get { return eventId; } }
        public string EventName { get { return eventName; } set { eventName = value; } }
        public DateTime EventDate { get { return eventDate; } set { eventDate = value; } }
        public string Category { get { return category; } set { category = value; } }
        public string Location { get { return location; } set { location = value; } }
        public bool IsActive { get { return isActive; } set { isActive = value; } }
        public Organization Organization { get { return belongsToOrg; } set { belongsToOrg = value; } }
        public PackIconKind AllowBettingAsIcon { get; private set; }
        public string AllowBettingAsString { get; private set; }
    }

    public class Bet
    {
        int betsId;
        DateTime betsDate;
        float odds; 
        int amount;
        int bettorsId; 
        bool status; 
        Event assignedEvent;
        public Bet(int betsId, DateTime betsDate, float odds, int amount, int bettorsId, bool status, Event assignedEvent) 
        {
            this.betsId = betsId;
            this.betsDate = betsDate;
            this.odds = odds;
            this.amount = amount;
            this.bettorsId = bettorsId;
            this.status = status;
            this.assignedEvent = assignedEvent;
        }
        public int BetsID { get { return betsId; } }
        public DateTime BetDate { get { return betsDate; } }
        public float Odds { get { return odds; } }
        public int Amount {  get { return amount; } }
        public int BettorsID { get { return bettorsId; } }
        public bool Status { get { return status; } }
        public Event Event { get { return assignedEvent; } }
    }

}
