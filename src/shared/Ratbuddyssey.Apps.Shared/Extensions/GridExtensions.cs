using System;
using System.Globalization;
using System.Linq;
#if WPF_APP
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#nullable enable

namespace Ratbuddyssey.Extensions
{
    public static class GridExtensions
    {
        #region ColumnsAndRows

        public static readonly DependencyProperty ColumnsAndRowsProperty =
            DependencyProperty.RegisterAttached(
                nameof(ColumnsAndRowsProperty).Replace("Property", string.Empty),
                typeof(string),
                typeof(GridExtensions),
                new PropertyMetadata(string.Empty, OnColumnsAndRowsChanged));

        public static string? GetColumnsAndRows(DependencyObject element)
        {
            return (string?)element.GetValue(ColumnsAndRowsProperty);
        }

        public static void SetColumnsAndRows(DependencyObject element, string? value)
        {
            element.SetValue(ColumnsAndRowsProperty, value);
        }

        private static void OnColumnsAndRowsChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not Grid grid)
            {
                throw new ArgumentException($"Element should be {nameof(Grid)}.");
            }
            if (args.NewValue is not string columnsAndRows)
            {
                throw new ArgumentException($"Value should be {nameof(String)}.");
            }

            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            if (string.IsNullOrWhiteSpace(columnsAndRows))
            {
                return;
            }

            var values = columnsAndRows.Split(';');
            foreach (var constraint in (values.ElementAtOrDefault(0) ?? "*")
                .Split(',')
                .Select(Constraint.Parse))
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = constraint.Value,
                    MinWidth = constraint.MinValue,
                    MaxWidth = constraint.MaxValue,
                });
            }
            foreach (var constraint in (values.ElementAtOrDefault(1) ?? "*")
                .Split(',')
                .Select(Constraint.Parse))
            {
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = constraint.Value,
                    MinHeight = constraint.MinValue,
                    MaxHeight = constraint.MaxValue,
                });
            }
        }

        public class Constraint
        {
            public GridLength Value { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; } = double.PositiveInfinity;

            public static Constraint Parse(string text)
            {
                text = text ?? throw new ArgumentNullException(nameof(text));

                var valueString = text.Contains('[')
                    ? text.Substring(0, text.IndexOf('['))
                    : text;
                var minMaxString = text.Contains('[')
                    ? text.Substring(text.IndexOf('[') + 1).TrimEnd(']')
                    : string.Empty;
                var minString = minMaxString.Contains('-')
                    ? minMaxString.Substring(0, minMaxString.IndexOf('-'))
                    : minMaxString;
                var maxString = minMaxString.Contains('-')
                    ? minMaxString.Substring(minMaxString.IndexOf('-') + 1)
                    : string.Empty;

                return new Constraint
                {
                    Value = GridLengthConverter.ConvertFromInvariantString(valueString),
                    MinValue = double.TryParse(
                        minString,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var minValue)
                        ? minValue
                        : 0.0,
                    MaxValue = double.TryParse(
                        maxString,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var maxValue)
                        ? maxValue
                        : double.PositiveInfinity,
                };
            }
        }

        #endregion

        #region Columns

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(
                nameof(ColumnsProperty).Replace("Property", string.Empty),
                typeof(string),
                typeof(GridExtensions),
                new PropertyMetadata(string.Empty, OnColumnsChanged));

        public static string? GetColumns(DependencyObject element)
        {
            return (string?)element.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject element, string? value)
        {
            element.SetValue(ColumnsProperty, value);
        }

        private static void OnColumnsChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not Grid)
            {
                throw new ArgumentException($"Element should be {nameof(Grid)}.");
            }
            if (args.NewValue is not string columns)
            {
                throw new ArgumentException($"Value should be {nameof(String)}.");
            }

            element.SetValue(ColumnsAndRowsProperty, $"{columns};*");
        }

        #endregion

        #region Rows

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached(
                nameof(RowsProperty).Replace("Property", string.Empty),
                typeof(string),
                typeof(GridExtensions),
                new PropertyMetadata(string.Empty, OnRowsChanged));

        public static string? GetRows(DependencyObject element)
        {
            return (string?)element.GetValue(RowsProperty);
        }

        public static void SetRows(DependencyObject element, string? value)
        {
            element.SetValue(RowsProperty, value);
        }

        private static void OnRowsChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not Grid)
            {
                throw new ArgumentException($"Element should be {nameof(Grid)}.");
            }
            if (args.NewValue is not string rows)
            {
                throw new ArgumentException($"Value should be {nameof(String)}.");
            }

            element.SetValue(ColumnsAndRowsProperty, $"*;{rows}");
        }

        #endregion

        #region GridLengthConverter

        /// <summary>
        /// https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/GridLengthConverter.cs
        /// </summary>
        private class GridLengthConverter
        {
            public static GridLength ConvertFromInvariantString(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return new GridLength(1, GridUnitType.Star);
                }
                if (text.ToUpperInvariant() is "AUTO" or "A")
                {
                    return GridLength.Auto;
                }

                if (text.Contains("*"))
                {
                    var value = text.Replace("*", string.Empty);

                    return new GridLength(
                        string.IsNullOrWhiteSpace(value)
                            ? 1
                            : Convert.ToDouble(value, CultureInfo.InvariantCulture),
                        GridUnitType.Star);
                }

                return new GridLength(Convert.ToDouble(text, CultureInfo.InvariantCulture));
            }
        }

        #endregion
    }
}