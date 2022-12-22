using System;
using System.Linq;
using System.Windows.Markup;
using System.ComponentModel;

namespace FileBatchRenamer
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension() { }
        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType == null || !enumType.IsEnum)
                return;

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var values = Enum.GetValues(EnumType).Cast<object>();

            var valuesAndDescriptions = from value in values
                                        select new
                                        {
                                            Value = value,
                                            Description = value.GetType()
                                                    .GetMember(value.ToString())[0]
                                                    .GetCustomAttributes(true)
                                                    .OfType<DescriptionAttribute>()
                                                    .First()
                                                    .Description
                                        };
            return valuesAndDescriptions.ToArray();
        }
    }
}