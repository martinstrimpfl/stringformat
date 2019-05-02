using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StringFormat
{

    public class CompositeFormatStringDescription
    {
        public CompositeFormatStringDescription(string template, List<string> placeholders)
        {

            CompositeFormatString = template;
            OrderedObjectNameList = placeholders.AsReadOnly();
        }

        public string CompositeFormatString { get; }

        public IList<string> OrderedObjectNameList { get; }
    }


    public class FormatStringConverter
    {
        public static CompositeFormatStringDescription Convert(string interpolatedFormatValue)
        {
            var processor = new Reader(interpolatedFormatValue);
            return processor.Process();
        }

        private  class Reader
        {
            private Action<char> action;

            private List<string> placeholders = new List<string>();

            private int positionInput;
            private int positionPlaceholder;
            private int positionOutput;

            private string inputString;
            private char[] placeholderBuffer;
            private char[] outputBuffer;

            public Reader(string template)
            {
                inputString = template;
                placeholderBuffer = new char[template.Length];
                outputBuffer = new char[template.Length + 10];
                action = ProcessNormal;
            }

            public CompositeFormatStringDescription Process()
            {
                while (positionInput < inputString.Length)
                {
                    action(inputString[positionInput++]);
                }

                return
                    new CompositeFormatStringDescription(
                        new string(outputBuffer.Take(positionOutput).ToArray()),
                        placeholders);
            }

            private void ProcessNormal(char inputChar)
            {
                outputBuffer[positionOutput++] = inputChar;

                if (inputChar == '{')
                {
                    positionPlaceholder = 0;
                    action = ProcessPlaceholder;
                }
            }

            private void ProcessPlaceholder(char inputChar)
            {
                var isEndChar = inputChar == '}' || inputChar == ',' || inputChar == ':';
                var isStartChar = inputChar == '{';

                if (isEndChar || isStartChar)
                {
                    if (positionPlaceholder > 0)
                    {
                        var placeholder = new string(placeholderBuffer.Take(positionPlaceholder).ToArray());

                        if (isEndChar)
                        {
                            placeholder = StorePlaceholderAndGetIndexAsString(placeholder);
                        }

                        foreach (var placeholderChar in placeholder)
                        {
                            outputBuffer[positionOutput++] = placeholderChar;
                        }
                    }
                    outputBuffer[positionOutput++] = inputChar;
                    action = ProcessNormal;
                    return;
                }

                placeholderBuffer[positionPlaceholder++] = inputChar;
            }

            private string StorePlaceholderAndGetIndexAsString(string placeholder)
            {
                var index =
                    placeholders
                        .FindIndex(existing => existing.Equals(placeholder, StringComparison.OrdinalIgnoreCase));

                if (index < 0)
                {
                    placeholders.Add(placeholder);
                    index = placeholders.Count - 1;
                }

                return index.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
