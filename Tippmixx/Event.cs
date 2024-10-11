using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tippmixx
{
    using System;
    using System.Collections.ObjectModel;
    using MySql.Data.MySqlClient;

    public class EventManager
    {
    }

    public class Event
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
    }

    public class Bet
    {
        public int BetsID { get; set; }
        public DateTime BetDate { get; set; }
        public float Odds { get; set; }
        public int Amount { get; set; }
        public int BettorsID { get; set; }
        public bool Status { get; set; }
    }

}
