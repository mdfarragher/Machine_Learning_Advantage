using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

namespace lesson_5
{
    /// <summary>
    /// The CalendarItem structure represents one item in a calendar.
    /// </summary>
    class CalendarItem
    {
        /// <summary>
        /// The subject of the meeting or event.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The location where to hold the meeting or event.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The date and time of the meeting.
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// The name of the guest to meet.
        /// </summary>
        public string Guest { get; set; }

        /// <summary>
        /// Reschedule the appointment.
        /// </summary>
        /// <param name="date">The new date for the appointment.</param>
        public void Reschedule(string date)
        {
            DateTime = date;
        }

        /// <summary>
        /// Return the string representation of the calendar item.
        /// </summary>
        /// <returns>The string representation of the calendar item.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0}]", Subject);
            if (Location != null)
                sb.AppendFormat(" at [{0}]", Location);
            if (DateTime != null)
                sb.AppendFormat(" on [{0}]", DateTime);
            if (Guest != null)
                sb.AppendFormat(" with [{0}]", Guest);
            return sb.ToString();
        }
    }

    /// <summary>
    /// The main program class.
    /// </summary>
    class Program
    {
        // *******************************************
        // PUT YOUR LUIS API KEY, URL, AND APP ID HERE
        // *******************************************

        // LUIS API credentials
        private const string LUIS_KEY = "...";
        private const string LUIS_API = "...";
        private const string LUIS_ID = "...";

        /// <summary>
        /// The main calendar.
        /// </summary>
        private static List<CalendarItem> calendar = new List<CalendarItem>();

        /// <summary>
        /// Add an entry to the calendar.
        /// </summary>
        /// <param name="subject">The subject of the meeting.</param>
        /// <param name="location">The location of the meeting.</param>
        /// <param name="date">The date and time of the meeting.</param>
        /// <param name="guest">The guest to meet.</param>
        private static void AddToCalendar(string subject, string location, string date, string guest)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // remove this line when done
            Console.WriteLine("Sorry, adding appointments is not implemented yet.");
        }

        /// <summary>
        /// Delete an entry from the calendar.
        /// </summary>
        /// <param name="subject">The subject of the meeting.</param>
        /// <param name="location">The location of the meeting.</param>
        /// <param name="date">The date and time of the meeting.</param>
        /// <param name="guest">The guest to meet.</param>
        private static void DeleteFromCalendar(string subject, string location, string date, string guest)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // remove this line when done
            Console.WriteLine("Sorry, deleting appointments is not implemented yet.");
        }

        /// <summary>
        /// Edit an entry in the calendar.
        /// </summary>
        /// <param name="subject">The subject of the meeting.</param>
        /// <param name="location">The location of the meeting.</param>
        /// <param name="date">The date and time of the meeting.</param>
        /// <param name="guest">The guest to meet.</param>
        private static void EditCalendar(string subject, string location, string date, string guest)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // remove this line when done
            Console.WriteLine("Sorry, editing appointments is not implemented yet.");
        }

        /// <summary>
        /// Find an entry in the calendar.
        /// </summary>
        /// <param name="subject">The subject of the meeting.</param>
        /// <param name="location">The location of the meeting.</param>
        /// <param name="date">The date and time of the meeting.</param>
        /// <param name="guest">The guest to meet.</param>
        private static void FindInCalendar(string subject, string location, string date, string guest)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // remove this line when done
            Console.WriteLine("Sorry, searching for appointments is not implemented yet.");
        }

        /// <summary>
        /// Check a person's availability.
        /// </summary>
        /// <param name="guest">The guest to meet.</param>
        private static void CheckAvailability(string guest)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // remove this line when done
            Console.WriteLine("Sorry, checking for availability is not implemented yet.");
        }

        /// <summary>
        /// The main program entry point.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        private static void Main(string[] args)
        {
            // create the LUIS client object
            var client = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(LUIS_KEY))
            {
                Endpoint = LUIS_API
            };

            // start the query loop
            Console.WriteLine("Hi Mark, how can I help you?");
            Console.WriteLine("(you can type 'exit' at any time to leave this conversation)");
            while (true)
            {
                // get the user query
                Console.Write(">> ");
                var query = Console.ReadLine();

                // abort if the user typed 'exit'
                if (query.ToLower() == "exit")
                    return;

                // ******************
                // ADD YOUR CODE HERE
                // ******************
            }
        }
    }
}
