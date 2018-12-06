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
        // LUIS API credentials
        private const string LUIS_KEY = "fe9398803aa2477da63b966d28f3f5f4";
        private const string LUIS_API = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/e2b0b019-de8e-4696-90dd-211e752d09de?subscription-key=fe9398803aa2477da63b966d28f3f5f4&timezoneOffset=-360&q=";
        private const string LUIS_ID = "e2b0b019-de8e-4696-90dd-211e752d09de";

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
            // add the meeting to the calendar
            var item = new CalendarItem()
            {
                Subject = subject,
                Location = location,
                DateTime = date,
                Guest = guest
            };
            calendar.Add(item);

            // notify user
            Console.WriteLine("Okay Mark, I've added the following meeting to your calendar:");
            Console.WriteLine(item);
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
            // find the specified appointment
            var item = from appointment in calendar
                       where (subject == null || appointment.Subject == subject)
                       && (location == null || appointment.Location == location)
                       && (date == null || appointment.DateTime == date)
                       && (guest == null || appointment.Guest == guest)
                       select appointment;

            // delete the appointment
            if (item.Count() > 0)
            {
                Console.WriteLine($"Okay Mark, I've removed the following appointment from your calendar:\r\n{item.First()}");
                calendar.Remove(item.First());
            }
            else
                Console.WriteLine("You do not have any matching appointments.");
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
            // find the specified appointment
            var item = from appointment in calendar
                       where appointment.Subject == subject
                       select appointment;

            // reschedule the appointment
            if (item.Count() > 0)
            {
                var appointment = item.First();
                appointment.Reschedule(date);
                Console.WriteLine($"Okay Mark, I've rescheduled [{appointment.Subject}] to {date}");
            }
            else
                Console.WriteLine("You do not have any matching appointments.");
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
            // find the specified appointment
            var item = from appointment in calendar
                       where (subject == null || appointment.Subject == subject)
                       && (location == null || appointment.Location == location)
                       && (date == null || appointment.DateTime == date)
                       && (guest == null || appointment.Guest == guest)
                       select appointment;

            // notify user
            if (item.Count() > 0)
                Console.WriteLine($"I found the following appointment in your calendar:\r\n{item.First()}");
            else
                Console.WriteLine("You do not have any matching appointments.");
        }

        /// <summary>
        /// Check a person's availability.
        /// </summary>
        /// <param name="guest">The guest to meet.</param>
        private static void CheckAvailability(string guest)
        {
            // find all appointments for the guest
            var blockedTimes = from appointment in calendar
                       where appointment.Guest == guest
                       select appointment.DateTime;

            // show results
            if (blockedTimes.Count() > 0)
            {
                string str = string.Join(", ", blockedTimes);
                Console.WriteLine($"{guest} is available all week, except {str}");
            }
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

                // call the language model and get a prediction
                var result = client.Prediction.ResolveAsync(LUIS_ID, query).Result;

                // get the meeting subject
                var subject = (from e in result.Entities
                               where e.Type == "Calendar.Subject"
                               select e.Entity).FirstOrDefault();

                // get the meeting location
                var location = (from e in result.Entities
                                where e.Type == "Calendar.Location"
                                select e.Entity).FirstOrDefault();

                // get the date and time of the meeting
                var date = (from e in result.Entities
                                where e.Type.StartsWith("builtin.datetimeV2")
                                select e.Entity).FirstOrDefault();

                // get the guest to meet
                var guest = (from e in result.Entities
                             where e.Type == "builtin.personName"
                             select e.Entity).FirstOrDefault();

                // handle intent
                switch (result.TopScoringIntent.Intent)
                {
                    case "Calendar.Add":
                        AddToCalendar(subject, location, date, guest);
                        break;
                    case "Calendar.Delete":
                        DeleteFromCalendar(subject, location, date, guest);
                        break;
                    case "Calendar.Edit":
                        EditCalendar(subject, location, date, guest);
                        break;
                    case "Calendar.Find":
                        FindInCalendar(subject, location, date, guest);
                        break;
                    case "Calendar.CheckAvailability":
                        CheckAvailability(guest);
                        break;
                    default:
                        Console.WriteLine("I'm sorry, I didn't get that.");
                        break;
                }
            }
        }
    }
}
