using System;

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

            Console.WriteLine(compositeMessage);
            Console.WriteLine(interpolatedMessage);
        }
    }

    public class MeetingTime
    {
        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
