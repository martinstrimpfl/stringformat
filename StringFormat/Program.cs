using System;
using System.Collections.Generic;
using System.Linq;

namespace StringFormat
{
    public enum Tenant
    {
        TenantA,
        TenantB
    }
    class Program
    {
        static void Main(string[] args)
        {

            List<int> numbers = new List<int> { 1, 6, 9 };

            var format = "There {0:0>is no numbers|1>is one number|2>are two numbers|3>are # numbers}.";

            Console.WriteLine(string.Format(new CollectionLengthFormatter(), format, numbers));


            var meetingTime =
                new MeetingTime
                {
                    Day = DayOfWeek.Monday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0)
                };

            var meetingTimeInConflict =
                new MeetingTime
                {
                    Day = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0)
                };

            var error =
                new MeetingError
                {
                    Meeting = meetingTime,
                    MeetingInConflict = meetingTimeInConflict
                };
           

            var interpolatedFormat = "The meeting {Meeting} is in a conflict with {MeetingInConflict}.";
            var compositeDescription = FormatStringConverter.Convert(interpolatedFormat);

            var objectAsDictionary = ObjectConverter.ConvertObject(error);

            var orderedObjectNameList = compositeDescription.OrderedObjectNameList;
            var argsToFormat = 
                objectAsDictionary
                    .Where(k => orderedObjectNameList.Contains(k.Key, StringComparer.OrdinalIgnoreCase))
                    .OrderBy(k => orderedObjectNameList.IndexOf(k.Key))
                    .Select(k => k.Value)
                    .ToArray();

            var errorMessageTenantA =
                string.Format(
                    new CustomFormatter(Tenant.TenantA), 
                    compositeDescription.CompositeFormatString, 
                    argsToFormat);

            var errorMessageTenantB =
                string.Format(
                    new CustomFormatter(Tenant.TenantB),
                    compositeDescription.CompositeFormatString,
                    argsToFormat);

            Console.WriteLine(errorMessageTenantA);
            Console.WriteLine(errorMessageTenantB);
        }
    }

    public class MeetingTime
    {
        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }

    public class MeetingError
    {
        public MeetingTime Meeting { get; set; }

        public MeetingTime MeetingInConflict { get; set; }
    }

    public class CustomFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly string timeFormat;

        public CustomFormatter(Tenant tenant)
        {
            switch (tenant)
            {
                case Tenant.TenantA:
                    timeFormat = "hh:mm";
                    break;

                case Tenant.TenantB:
                    timeFormat = "HH:mm tt";
                    break;
            }
        }

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
            return DateTime.Today.Add(time).ToString(timeFormat, this);
        }
    }
}
