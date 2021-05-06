using System;
using Audyssey.ViewModels;
using Icon = MaterialDesignThemes.Wpf.PackIconKind;

#nullable enable

namespace Ratbuddyssey.Converters
{
    public class TypeToIconConverter : BaseConverter<Type?, Icon>
    {
        /// <summary>
        /// https://materialdesignicons.com/.
        /// </summary>
        public static Icon Convert(Type? type)
        {
            if (type == typeof(FileViewModel))
            {
                return Icon.File;
            }
            if (type == typeof(EthernetViewModel))
            {
                return Icon.EthernetCable;
            }

            return Icon.None;
        }

        public TypeToIconConverter() : base(Convert)
        {
        }
    }
}
