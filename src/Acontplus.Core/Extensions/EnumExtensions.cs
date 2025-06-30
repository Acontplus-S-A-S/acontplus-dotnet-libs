using System.ComponentModel;

namespace Acontplus.Core.Extensions;

public static class EnumExtensions
{
    public static string DisplayName(this Enum value)
    {
        var type = value.GetType();

        var memInfo = type.GetMember(value.ToString());

        switch (memInfo.Length)
        {
            case > 0:
                {
                    var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs.Length > 0)
                    {
                        return ((DescriptionAttribute)attrs[0]).Description;
                    }

                    break;
                }
        }

        return value.ToString();
    }
}
