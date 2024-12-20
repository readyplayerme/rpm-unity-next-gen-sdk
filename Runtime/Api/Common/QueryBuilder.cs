using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;

namespace ReadyPlayerMe
{
    public static class QueryBuilder
    {
        public static string BuildQueryString(object queryParams)
        {
            var properties = queryParams.GetType().GetProperties()
                .Where(prop => prop.GetValue(queryParams, null) != null)
                .ToDictionary(
                    GetPropertyName,
                    prop => prop.GetValue(queryParams, null));

            if (properties.Count == 0)
                return string.Empty;

            var queryString = new StringBuilder();
            queryString.Append('?');

            foreach (var (key, value) in properties)
            {
                if (value is IDictionary dictionary)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        queryString.Append(
                            $"{Uri.EscapeDataString(entry.Key.ToString())}={Uri.EscapeDataString(entry.Value.ToString())}&");
                    }
                }
                else if (value is IEnumerable<string> stringArray)
                {
                    foreach (var item in stringArray)
                    {
                        queryString.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(item)}&");
                    }
                }
                else
                {
                    queryString.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value.ToString())}&");
                }
            }

            // Remove the trailing '&' and return the query string
            return queryString.ToString().TrimEnd('&');
        }
        
        private static string GetPropertyName(MemberInfo prop)
        {
            return prop.GetCustomAttribute<JsonPropertyAttribute>() != null
                ? prop.GetCustomAttribute<JsonPropertyAttribute>().PropertyName
                : prop.Name;
        }
    }
    
    
}