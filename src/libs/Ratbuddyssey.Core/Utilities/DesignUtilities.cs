using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Audyssey.Utilities
{
    public static class DesignUtilities
    {
        public static T Generate<T>(
            int index = 1, 
            IReadOnlyDictionary<string, Func<int, object>>? patterns = null) 
            where T : new()
        {
            patterns ??= new Dictionary<string, Func<int, object>>();

            var obj = new T();
            foreach(var property in typeof(T).GetProperties().Where(property => property.CanWrite))
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = patterns.TryGetValue(property.Name, out var func)
                        ? func(index)
                        : $"{property.Name}{index}";
                    property.SetValue(obj, value);
                }
                else if (property.PropertyType == typeof(int))
                {
                    var value = patterns.TryGetValue(property.Name, out var func)
                        ? func(index)
                        : DesignRandom.Next();
                    property.SetValue(obj, value);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(obj, DesignRandom.NextDouble() > 0.5);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(obj, Guid.NewGuid());
                }
                else if (property.PropertyType.IsEnum)
                {
                    var values = Enum.GetValues(property.PropertyType);
                    var value = values.GetValue(DesignRandom.Next(values.Length));

                    property.SetValue(obj, value);
                }
            }

            return obj;
        }

        public static IReadOnlyCollection<T> GenerateCollection<T>(
            int count = 50, 
            IReadOnlyDictionary<string, Func<int, object>>? patterns = null) 
            where T : new()
        {
            return Enumerable
                .Range(1, count)
                .Select(i => Generate<T>(i, patterns))
                .ToArray();
        }

        public static IReadOnlyCollection<T> GenerateCollection<T>(
            Func<int, T> pattern,
            int count = 50)
        {
            return Enumerable
                .Range(1, count)
                .Select(pattern)
                .ToArray();
        }

        public static Random DesignRandom { get; } = new();

        public static Func<int, object> EmailPattern { get; } = i => $"email{i}@mail.com";
        public static Func<int, object> LoremIpsumPattern { get; } = _ => "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec molestie dictum tortor, ac ultricies magna tincidunt sed. Proin pharetra porta nisi, ut vestibulum diam dignissim et. Vestibulum nisl urna, aliquam vel dolor vitae, volutpat elementum ante. Curabitur ex metus, auctor eget sapien id, ornare pellentesque turpis. Maecenas bibendum elit in lectus bibendum elementum. In hac habitasse platea dictumst. Nunc et feugiat turpis. Suspendisse ex dolor, mattis vitae pellentesque nec, tempor nec diam. Sed porttitor cursus pellentesque. Nam blandit ante sed suscipit gravida. Cras lobortis aliquam tortor, ac iaculis nulla sagittis a. Suspendisse luctus, mi interdum faucibus feugiat, elit tellus luctus ex, at dictum velit felis sit amet mi.";
        public static Func<int, object> DateStringPattern { get; } = _ => $"{new DateTime(DateTime.Now.Ticks - DesignRandom.Next() * 1_000_000L)}";
        public static Func<int, object> DoubleStringPattern { get; } = _ => $"{GetDouble():F2}";
        public static Func<int, object> BalanceStringPattern { get; } = _ => $"${GetDouble(-100.0, 100.0):F2}";
        public static Func<int, object> MultilinePattern { get; } = _ => $@"Multiline Text
Multiline Text
Multiline Text
Multiline Text
Multiline Text
Multiline Text";

        public static double GetDouble(double from = 0.0, double to = 1.0)
        {
            return from + (to - from) * DesignRandom.NextDouble();
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> objects)
        {
            var table = new DataTable();

            var properties = typeof(T).GetProperties()
                .Where(property => property.CanRead)
                .ToArray();

            foreach (var property in properties)
            {
                table.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (var values in objects
                .Select(value => properties.Select(property => property.GetValue(value))
                .ToArray()))
            {
                table.Rows.Add(values);
            }

            return table;
        }

        public static DataTable ToDataTable(this IEnumerable objects)
        {
            objects = objects ?? throw new ArgumentNullException(nameof(objects));

            var table = new DataTable();

            PropertyInfo[]? properties = null;
            var enumerator = objects.GetEnumerator();

            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                var value = enumerator.Current;
                if (properties == null)
                {
                    properties = value
                        .GetType()
                        .GetProperties()
                        .Where(property => property.CanRead)
                        .ToArray();

                    foreach (var property in properties)
                    {
                        table.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                    }
                }

                table.Rows.Add(properties
                    .Select(property => property.GetValue(value))
                    .ToArray());
            }

            return table;
        }
    }
}
