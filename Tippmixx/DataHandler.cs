using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tippmixx
{
    public class DataHandler
    {
        static MySqlConnection connection;
        static DataHandler instance = new DataHandler("Server=localhost;Database=tippmix;User ID=root;Password=;");
        DataHandler(string connectionString) 
        { 
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }
        ~DataHandler() {
            connection.Close();
        }
        public static bool RequestWithoutQuery(string command) 
        {
            return true;
        }
        public static List<T> RequestWithQuery<T>(string command, Dictionary<string, string> argumentDefiner, Dictionary<string, Type> typeDefiner) 
        {
            List<T> toReturn = new List<T>();
            int index = 0;
            Type[] typeArray = new Type[typeDefiner.Count];
            foreach (KeyValuePair<string, Type> kvp in typeDefiner)
            {
                typeArray[index] = kvp.Value;
                index++;
            }
            ConstructorInfo ctor = typeof(T).GetConstructor(typeArray);
            ParameterInfo[] parameters = ctor.GetParameters();
            foreach (KeyValuePair<string, string> kvp in argumentDefiner)
            {

            }
            using (MySqlCommand cmd = new MySqlCommand(command, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        
                        T final = (T)Activator.CreateInstance(typeof(T), parameters);
                        toReturn.Add(final);
                    }
                }
            }
            return toReturn;
        }
    }
}
