using System;
using System.Collections.Generic;
using System.Linq;

namespace StringFormat
{
    public class CompositeFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly List<ICustomFormatter> formatters;

        public CompositeFormatter(params ICustomFormatter[] formatters)
        {
            this.formatters = formatters.ToList();
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            foreach (var formatter in formatters)
            {
                var result = formatter.Format(format, arg, formatProvider);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }
    }
}
