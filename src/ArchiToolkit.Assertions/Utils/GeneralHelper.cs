using System.Text.RegularExpressions;

namespace ArchiToolkit.Assertions.Utils;

internal static class GeneralHelper
{
    public static string ReplacePlaceHolder(this string input, string placeholder, string replacementValue)
    {
        var pattern = $@"\{{({placeholder})(:.*?)?\}}";
        var replacement = $"{{{replacementValue}$2}}";
        return Regex.Replace(input, pattern, replacement);
    }

    public static string GetFullName(this Type type)
    {
        if (!type.IsGenericType)
            return GetTypeName(type);

        var genericArgs = string.Join(", ", type.GetGenericArguments()
            .Select(GetFullName));

        var typeName = GetTypeName(type.GetGenericTypeDefinition());
        typeName = typeName[..typeName.IndexOf('`')];

        return $"{typeName}<{genericArgs}>";

        static string GetTypeName(Type type) => type.FullName ?? type.Name;
    }
}