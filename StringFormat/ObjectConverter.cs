using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringFormat
{
    public class ObjectConverter
    {
        public static Dictionary<string, dynamic> ConvertObject(dynamic value)
        {
            var objectsToFormat = new Dictionary<string, dynamic>();
            GetAggregatedDomainObjectsTraversingRecursively(string.Empty, value, objectsToFormat);
            return objectsToFormat;
        }

        // TODO update to only include properties specified by the interface, if used
        private static void GetAggregatedDomainObjectsTraversingRecursively<T>(
            string previousName,
            T someObject,
            Dictionary<string, dynamic> values)
        {
            if (someObject == null)
            {
                return;
            }

            var propertyInfoList =
                someObject
                .GetType()
                .GetProperties()
                .Where(propertyInfo => propertyInfo.DeclaringType == someObject.GetType())
                .ToList();

            // Iterate through all the properties
            foreach (var property in propertyInfoList)
            {
                dynamic item = property.GetValue(someObject);

                if (item != null)
                {
                    var key = string.IsNullOrEmpty(previousName) ? property.Name : previousName + "." + property.Name;
                    values.Add(key, item);

                    if ((property.PropertyType.IsClass || property.PropertyType.IsInterface)
                        &&
                        !(
                            property.PropertyType.IsGenericType
                            &&
                            (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                                ||
                             property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                        &&
                        property.PropertyType != typeof(string)
                        &&
                        !property.PropertyType.GetInterfaces().Any(ty => ty is IEnumerable))
                    {
                        // Recursive call to scan through and find child object properties
                        // ignore lists, dictionaries and similar types
                        GetAggregatedDomainObjectsTraversingRecursively(key, item, values);
                    }
                }
            }
        }
    }
}
