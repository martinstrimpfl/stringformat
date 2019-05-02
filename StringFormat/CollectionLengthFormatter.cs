using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StringFormat
{
    public  class CollectionLengthFormatter : IFormatProvider, ICustomFormatter
    {

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var argumentCollection = arg as ICollection;

            if (argumentCollection == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(format))
            {
                return null;
            }

            var parts = format.Split('|');

            var partDictionary = new Dictionary<int, string>();

            for (var i = 0; i < parts.Length; i++)
            {
                var localParts = parts[i].Split('>');

                if (localParts.Length != 2)
                {
                    return null;
                }

                int number;

                if (!int.TryParse(localParts[0], out number) || string.IsNullOrEmpty(localParts[1]))
                {
                    return null;
                }

                partDictionary.Add(number, localParts[1]);
            }

            var count = argumentCollection.Count;

            for (var i = partDictionary.Keys.Count - 1; i >= 0; i--)
            {
                var key = partDictionary.Keys.ElementAt(i);
                if (count >= key)
                {
                    return partDictionary[key].Replace("#", count.ToString(CultureInfo.InvariantCulture));
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
