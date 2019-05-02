using System;
using System.Collections.Generic;
using System.Linq;

namespace StringFormat
{
    class Program
    {
        static void Main(string[] args)
        {

            var meetingTime =
                new MeetingTime
                {
                    Day = DayOfWeek.Monday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0)
                };

            var compositeFormat = "The meeting {0}: {1} - {2} is in a conflict.";

            var compositeMessage = string.Format(compositeFormat, meetingTime.Day, meetingTime.StartTime, meetingTime.EndTime);

            var interpolatedMessage = $"The meeting {meetingTime.Day}: {meetingTime.StartTime} - {meetingTime.EndTime} is in a conflict.";

            var runtimeTokens = new Dictionary<string, string>
            {
                { "{Day}", meetingTime.Day.ToString() },
                { "{Start}", meetingTime.StartTime.ToString() },
                { "{End}", meetingTime.EndTime.ToString() }
            };

            var interpolatedStoredFormat = "The meeting {Day}: {Start} - {End} is in a conflict.";

            var interpolatedStoredMessage =
                runtimeTokens
                    .Aggregate(
                        interpolatedStoredFormat,
                        (current, token) => current.Replace(token.Key, token.Value));

            var compositeFormat2 = "The meeting {0} is in a conflict.";
            var compositeMessage2 = string.Format(new CustomFormatter(), compositeFormat2, meetingTime);

            Console.WriteLine(compositeMessage);
            Console.WriteLine(interpolatedMessage);
            Console.WriteLine(interpolatedStoredMessage);
            Console.WriteLine(compositeMessage2);
        }
    }

    public class MeetingTime
    {
        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }

    public class CustomFormatter : IFormatProvider, ICustomFormatter
    {

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return Format((dynamic)arg);
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }

        private string Format(MeetingTime meetingTime)
        {
            return string.Format(this, "{0}: {1} - {2}", meetingTime.Day, meetingTime.StartTime, meetingTime.EndTime);
        }

        private static string Format(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Friday: return "F";
                case DayOfWeek.Wednesday: return "W";
                case DayOfWeek.Monday: return "M";
                case DayOfWeek.Tuesday: return "TU";
                case DayOfWeek.Thursday: return "TH";
                case DayOfWeek.Saturday: return "SA";
                case DayOfWeek.Sunday: return "SU";
            }

            return "??";
        }

        private string Format(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("hh:mm", this);
        }
    }
}
