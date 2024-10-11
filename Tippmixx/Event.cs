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
        public static void CreateEvent(string eventName, DateTime eventDate, string category, string location)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                INSERT INTO Events (EventName, EventDate, Category, Location) 
                VALUES (@EventName, @EventDate, @Category, @Location);
                SELECT LAST_INSERT_ID();"; // Retrieve the generated EventID

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EventName", eventName);
                    cmd.Parameters.AddWithValue("@EventDate", eventDate);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Location", location);

                    int newEventID = Convert.ToInt32(cmd.ExecuteScalar()); // Execute the command and get the new EventID
                    Console.WriteLine("New Event created with ID: " + newEventID);
                }
            }
        }

        public static ObservableCollection<Event> RefreshEventList(string input = null)
        {
            ObservableCollection<Event> eventList = new();
            eventList.Clear();

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
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
                    query = $"SELECT EventID, EventName, EventDate, Category, Location FROM Events WHERE EventName LIKE '%{input}%'";
                }
                else
                {
                    query = @"
                    SELECT EventID, EventName, EventDate, Category, Location 
                    FROM Events";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventList.Add(new Event
                            {
                                EventID = Convert.ToInt32(reader["EventID"]),
                                EventName = reader["EventName"].ToString(),
                                EventDate = DateTime.Parse(reader["EventDate"].ToString()),
                                Category = reader["Category"].ToString(),
                                Location = reader["Location"].ToString()
                            });
                        }
                    }
                }
            }
            return eventList;
        }

        public static void PlaceBet(int bettorId, int eventId, float odds, int amount)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                INSERT INTO Bets (BetDate, Odds, Amount, BettorsID, EventID, Status) 
                VALUES (@BetDate, @Odds, @Amount, @BettorsID, @EventID, @Status);";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BetDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Odds", odds);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@BettorsID", bettorId);
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    cmd.Parameters.AddWithValue("@Status", true); // Assuming the status is true when placing a bet

                    cmd.ExecuteNonQuery(); // Execute the command to insert the bet
                    Console.WriteLine("Bet placed successfully for EventID: " + eventId);
                }
            }
        }

        public static ObservableCollection<Bet> GetBetsByEventId(int eventId)
        {
            ObservableCollection<Bet> betList = new();
            betList.Clear();

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=tippmix;User ID=root;Password=;"))
            {
                conn.Open();
                string query = @"
                SELECT BetsID, BetDate, Odds, Amount, BettorsID, Status 
                FROM Bets WHERE EventID = @EventID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            betList.Add(new Bet
                            {
                                BetsID = Convert.ToInt32(reader["BetsID"]),
                                BetDate = DateTime.Parse(reader["BetDate"].ToString()),
                                Odds = Convert.ToSingle(reader["Odds"]),
                                Amount = Convert.ToInt32(reader["Amount"]),
                                BettorsID = Convert.ToInt32(reader["BettorsID"]),
                                Status = (bool)reader["Status"]
                            });
                        }
                    }
                }
            }
            return betList;
        }
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
