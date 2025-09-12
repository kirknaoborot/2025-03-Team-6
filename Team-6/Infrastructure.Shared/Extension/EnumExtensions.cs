using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());
        if (memberInfo.Length > 0)
        {
            var displayAttribute = memberInfo[0]
                .GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute != null)
                return displayAttribute.Name;
        }
        return enumValue.ToString(); // если Display не задан
    }
}